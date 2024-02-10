namespace LightPadd.Core.Models.Options;

public class HubitatOptions
{
    public const string OptionsKey = "Hubitat";

    public string BaseUrl { get; set; } = null!;
    public ushort PostbackUrlPort { get; set; }
    public HubitatRoom[] Rooms { get; set; } = null!;
}

public record HubitatRoom(string Id, string AccessToken, string PostbackUrlPath);
