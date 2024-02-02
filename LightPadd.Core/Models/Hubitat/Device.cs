using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using PolyJson;

namespace LightPadd.Core.Models.Hubitat;

public class Device
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Room { get; set; } = null!;
    public BaseDeviceAttribute[] Attributes { get; set; } = null!;

    // This is an array that contains a mix of: CapabilityKind and CapabilityEntry.
    // Hard to figure out how to deserialize. We'll come back to this later.
    public JsonElement[] Capabilities { get; set; } = null!;
    public string[] Commands { get; set; } = null!;

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; } = null!;
}

[PolyJsonConverter("dataType")]
[PolyJsonConverter.SubType(typeof(StringDeviceAttribute), "STRING")]
[PolyJsonConverter.SubType(typeof(EnumDeviceAttribute), "ENUM")]
[PolyJsonConverter.SubType(typeof(NumberDeviceAttribute), "NUMBER")]
public abstract class BaseDeviceAttribute
{
    public string Name { get; set; } = null!;
    public DeviceAttributeDataTypeKind DataType { get; set; }
}

public class StringDeviceAttribute : BaseDeviceAttribute
{
    public string? CurrentValue { get; set; }
}

public class EnumDeviceAttribute : BaseDeviceAttribute
{
    public string? CurrentValue { get; set; }
    public string[] Values { get; set; } = null!;
}

public class NumberDeviceAttribute : BaseDeviceAttribute
{
    public double? CurrentValue { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeviceAttributeDataTypeKind
{
    [JsonPropertyName("STRING")]
    String,

    [JsonPropertyName("ENUM")]
    Enum,

    [JsonPropertyName("NUMBER")]
    Number
}

public class CapabilityEntry
{
    public CapabilityAttribute[] Attributes { get; set; } = null!;
}

public class CapabilityAttribute
{
    public string Name { get; set; } = null!;
    public string? DataType { get; set; }
}

public enum CapabilityKind
{
    AccelerationSensor,
    Actuator,
    AirQuality,
    Alarm,
    AudioNotification,
    AudioVolume,
    Battery,
    Beacon,
    Bulb,
    Button,
    CarbonDioxideMeasurement,
    CarbonMonoxideDetector,
    ChangeLevel,
    Chime,
    ColorControl,
    ColorMode,
    ColorTemperature,
    Configuration,
    Consumable,
    ContactSensor,
    CurrentMeter,
    DoorControl,
    DoubleTapableButton,
    EnergyMeter,
    EstimatedTimeOfArrival,
    FanControl,
    FilterStatus,
    Flash,
    GarageDoorControl,
    GasDetector,
    HealthCheck,
    HoldableButton,
    IlluminanceMeasurement,
    ImageCapture,
    Indicator,
    Initialize,
    LevelPreset,
    Light,
    LightEffects,
    LiquidFlowRate,
    LocationMode,
    Lock,
    LockCodes,
    MediaController,
    MediaInputSource,
    MediaTransport,
    Momentary,
    MotionSensor,
    MusicPlayer,
    Notification,
    Outlet,
    Polling,
    PowerMeter,
    PowerSource,
    PresenceSensor,
    PressureMeasurement,
    PushableButton,
    Refresh,
    RelativeHumidityMeasurement,
    RelaySwitch,
    ReleasableButton,
    SamsungTV,
    SecurityKeypad,
    Sensor,
    ShockSensor,
    SignalStrength,
    SleepSensor,
    SmokeDetector,
    SoundPressureLevel,
    SoundSensor,
    SpeechRecognition,
    SpeechSynthesis,
    StepSensor,
    Switch,
    SwitchLevel,
    TV,
    TamperAlert,
    Telnet,
    TemperatureMeasurement,
    TestCapability,
    Thermostat,
    ThermostatCoolingSetpoint,
    ThermostatFanMode,
    ThermostatHeatingSetpoint,
    ThermostatMode,
    ThermostatOperatingState,
    ThermostatSchedule,
    ThermostatSetpoint,
    ThreeAxis,
    TimedSession,
    Tone,
    TouchSensor,
    UltravioletIndex,
    Valve,
    Variable,
    VideoCamera,
    VideoCapture,
    VoltageMeasurement,
    WaterSensor,
    WindowBlind,
    WindowShade,
    ZwMultichannel,

    [JsonPropertyName("pHMeasurement")]
    PHMeasurement,
}
