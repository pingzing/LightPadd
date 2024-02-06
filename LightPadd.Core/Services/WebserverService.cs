using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using GenHTTP.Api.Infrastructure;
using GenHTTP.Api.Protocol;
using GenHTTP.Engine;
using GenHTTP.Modules.Conversion;
using GenHTTP.Modules.Conversion.Providers;
using GenHTTP.Modules.Functional;
using LightPadd.Core.Messaging;
using LightPadd.Core.Models.Hubitat;

namespace LightPadd.Core.Services
{
    public class WebserverService
    {
        private readonly IServerHost _host;
        private readonly IMessenger _messenger;

        public WebserverService(IMessenger messenger)
        {
            _messenger = messenger;

            var handler = _host = Host.Create();

            _host
                .Handler(
                    Inline
                        .Create()
                        .Post("/socketStatus", OnSocketStatusUpdate)
                        .Formats(
                            Serialization
                                .Empty()
                                .Add(ContentType.ApplicationJson, new SourceGenJsonFormat())
                                .Default(ContentType.ApplicationJson)
                        )
                )
                .Port(8080)
#if DEBUG
                .Development()
#endif
                .Start();
        }

        public void OnSocketStatusUpdate(StatusEventPayload payload)
        {
            string id = payload.Content.DeviceId;
            bool isOn = payload.Content.Value.Equals("on", StringComparison.OrdinalIgnoreCase);
            _messenger.Send<SwitchStateChangedArgs, string>(new(id, isOn), id);
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
