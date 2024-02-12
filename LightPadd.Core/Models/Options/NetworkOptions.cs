namespace LightPadd.Core.Models.Options;

public class NetworkOptions
{
    public const string OptionsKey = "Network";

    /// <summary>
    /// The first portions of the machine's IP address or hostname.
    /// Used to determine the correct IP address via .StartsWith() when
    /// setting the Postback URL to the Hubitat.<br/><br/>
    /// NOT CIDR-compliant. Simple string.
    /// </summary>
    public string NetworkPrefix { get; set; } = null!;
}
