using System.Diagnostics;
using System.Text.Json;
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
            _host.Handler(handler).Port(8080)
#if DEBUG
            .Development()
#endif
            .Start();
        }

        public async Task OnSocketStatusUpdate(StatusEventPayload payload)
        {
            // TODO: Fire some kind of event that the ButtonViewModel can hear, and update its state
            Debug.WriteLine(
                $"Received POST: {JsonSerializer.Serialize(payload, HubitatGenerationContext.Default.StatusEventPayload)}"
            );
        }
    }
}
