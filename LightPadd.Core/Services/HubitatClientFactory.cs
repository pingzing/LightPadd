using System.Net.Http;

namespace LightPadd.Core.Services;

public class HubitatClientFactory(IHttpClientFactory clientFactory)
{
    private readonly IHttpClientFactory _clientFactory = clientFactory;

    public HubitatClient Create(string roomId, string roomAccessToken)
    {
        var client = _clientFactory.CreateClient(roomId);
        return new HubitatClient(client, roomAccessToken);
    }
}
