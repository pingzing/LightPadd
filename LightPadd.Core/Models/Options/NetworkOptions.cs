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

    /// <summary>
    /// If <see langword="true"/>, will attempt to set the postback URL for each room
    /// to this machine's IP address.
    /// </summary>
    public bool SetPostbackUrl { get; set; }
}
