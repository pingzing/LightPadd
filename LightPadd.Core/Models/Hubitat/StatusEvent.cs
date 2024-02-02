using System.Text.Json.Serialization;

namespace LightPadd.Core.Models.Hubitat;

public class StatusEventPayload
{
    public StatusEvent Content { get; set; } = null!;
}

// TODO: Better typing here

/*
 * Example value:
 * {
    "Name": "switch",
    "Value": "off",
    "DisplayName": "Squiggly Light Plug",
    "DeviceId": "1",
    "DescriptionText": "Squiggly Light Plug was turned off [digital]",
    "Unit": null,
    "Data": null,
    "Type": "digital"
  }
 */
public class StatusEvent
{
    // for the hue plugs, this is "switch"
    // Maybe refers to the individual piece of the unit that
    // actually did something?
    // e.g. a smart plug might have "switch" and "power monitor" capability or something
    public string Name { get; set; } = null!;

    // typically "on" and "off"
    public string Value { get; set; } = null!;

    // The name that I set in hubitat
    public string DisplayName { get; set; } = null!;

    // the hubitat device ID under the maker API's context, as a string
    // usually plain integers starting at 1
    public string DeviceId { get; set; } = null!;

    // human-readable text
    public string DescriptionText { get; set; } = null!;
    public string? Unit { get; set; }
    public string? Data { get; set; }
    public string? Type { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(StatusEventPayload))]
internal partial class SourceGenerationContext : JsonSerializerContext { }
