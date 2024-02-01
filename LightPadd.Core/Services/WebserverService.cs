using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using GenHTTP.Api.Infrastructure;
using GenHTTP.Engine;
using GenHTTP.Modules.Functional;
using LightPadd.Core.Models.Hubitat;

namespace LightPadd.Core.Services
{
    public class WebserverService
    {
        private readonly IServerHost _host;

        public WebserverService()
        {
            var handler = Inline.Create().Post("/socketStatus", OnSocketStatusUpdate);

            _host = Host.Create();
            _host.Handler(handler)
#if DEBUG
            .Development()
#endif
            .Run();
        }

        public async Task OnSocketStatusUpdate(StatusEventPayload payload)
        {
            // TODO: Update some kind of stateful status keeper service
            Debug.WriteLine(
                "Received POST:",
                JsonSerializer.Serialize(
                    payload,
                    SourceGenerationContext.Default.StatusEventPayload
                )
            );
        }
    }
}
