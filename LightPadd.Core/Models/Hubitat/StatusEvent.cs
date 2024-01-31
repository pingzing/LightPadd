using System.Text.Json.Serialization;

namespace LightPadd.Core.Models.Hubitat;

public class StatusEventPayload
{
    public StatusEvent[] Data { get; set; } = null!;
}

public class StatusEvent
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string DeviceId { get; set; } = null!;
    public string DescriptionText { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public string Data { get; set; } = null!;
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(StatusEventPayload))]
internal partial class SourceGenerationContext : JsonSerializerContext { }
