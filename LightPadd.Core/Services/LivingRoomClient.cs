using System.Net.Http;

namespace LightPadd.Core.Services;

public class LivingRoomClient : HubitatClientBase
{
    public LivingRoomClient(HttpClient httpClient)
        : base(httpClient, "24220d64-8ef2-4b44-a815-a41e64c23c49") { }
}
