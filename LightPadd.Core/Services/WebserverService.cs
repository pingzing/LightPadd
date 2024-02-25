using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.Messaging;
using GenHTTP.Api.Infrastructure;
using GenHTTP.Api.Protocol;
using GenHTTP.Engine;
using GenHTTP.Modules.Conversion;
using GenHTTP.Modules.Conversion.Providers;
using GenHTTP.Modules.Functional;
using LightPadd.Core.Events;
using LightPadd.Core.Messaging;
using LightPadd.Core.Models.Hubitat;
using LightPadd.Core.Models.Options;
using Microsoft.Extensions.Options;

namespace LightPadd.Core.Services
{
    public class WebserverService
    {
        private readonly IServerHost _host;
        private readonly IMessenger _messenger;
        private readonly IOptionsMonitor<HubitatOptions> _hubitatOptions;
        private readonly SerializationBuilder _sourceGenJsonRegistry;
        private readonly Timer _debounceTimer = new();

        public WebserverService(
            IMessenger messenger,
            IOptionsMonitor<HubitatOptions> hubitatOptions
        )
        {
            _messenger = messenger;
            _hubitatOptions = hubitatOptions;
            _hubitatOptions.OnChange(
                (newOpts) =>
                {
                    _debounceTimer.Debounce(
                        () =>
                        {
                            OptionsChanged(newOpts);
                        },
                        TimeSpan.FromSeconds(1)
                    );
                }
            );

            _sourceGenJsonRegistry = Serialization
                .Empty()
                .Add(ContentType.ApplicationJson, new SourceGenJsonFormat())
                .Default(ContentType.ApplicationJson);
            _host = Host.Create();

            OptionsChanged(hubitatOptions.CurrentValue);
        }

        private void OptionsChanged(HubitatOptions options)
        {
            _host.Stop();

            var handlerBuilder = Inline.Create().Formats(_sourceGenJsonRegistry);
            foreach (var room in _hubitatOptions.CurrentValue.Rooms)
            {
                handlerBuilder = handlerBuilder.Post(
                    room.PostbackUrlPath,
                    (StatusEventPayload p) => OnSocketStatusUpdate(p, room.Id)
                );
            }

            _host
                .Handler(handlerBuilder)
                .Port(_hubitatOptions.CurrentValue.PostbackUrlPort)
#if DEBUG
                .Development()
#endif
                .Start();
        }

        public void OnSocketStatusUpdate(StatusEventPayload payload, string roomId)
        {
            string deviceId = payload.Content.DeviceId;
            bool isOn = payload.Content.Value.Equals("on", StringComparison.OrdinalIgnoreCase);
            _messenger.Send<SwitchStateChangedArgs, string>(
                new(deviceId, isOn),
                $"{roomId}-{deviceId}"
            );
        }

        private class SourceGenJsonFormat : ISerializationFormat
        {
            private static readonly JsonSerializerOptions _options =
                new()
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    TypeInfoResolver = HubitatPostGenerationContext.Default
                };

            public ValueTask<object?> DeserializeAsync(
                Stream stream,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type
            )
            {
                // The TypeInfoResolver above takes care of the warning, but the linter doesn't realize it.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
                return JsonSerializer.DeserializeAsync(stream, type, _options);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            }

            public ValueTask<IResponseBuilder> SerializeAsync(IRequest request, object response)
            {
                throw new NotImplementedException();
            }
        }
    }

    [JsonSerializable(typeof(StatusEventPayload))]
    internal partial class HubitatPostGenerationContext : JsonSerializerContext { }
}
