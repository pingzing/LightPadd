using System.Text.Json.Serialization;

namespace LightPadd.Core.Models.Hubitat;

// This is where we register all the types that System.Text.Json will need to know
// how to deserialize (specifically for Hubitat API stuff).
// This is needed if we want to use the source generator version of STJ (which we do).

[JsonSerializable(typeof(StatusEventPayload))]
[JsonSerializable(typeof(Device))]
[JsonSerializable(typeof(StringDeviceAttribute))]
[JsonSerializable(typeof(EnumDeviceAttribute))]
[JsonSerializable(typeof(NumberDeviceAttribute))]
internal partial class HubitatGenerationContext : JsonSerializerContext { }
