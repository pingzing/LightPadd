using System.Net.Http;

namespace LightPadd.Core.Services;

public class LivingRoomClient : HubitatClientBase
{
    // TODO: Probably more responsible to have this in a config file, and _not_ in source control
    public LivingRoomClient(HttpClient httpClient)
        : base(httpClient, "24220d64-8ef2-4b44-a815-a41e64c23c49") { }
}
