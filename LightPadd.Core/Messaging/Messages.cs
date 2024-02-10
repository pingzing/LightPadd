namespace LightPadd.Core.Messaging;

public record AppShutdownMessage(int ExitCode);

public record SwitchStateChangedArgs(string DeviceId, bool IsOn);
