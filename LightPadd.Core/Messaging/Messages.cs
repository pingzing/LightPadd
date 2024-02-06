namespace LightPadd.Core.Messaging;

public record AppShutdownMessage(int ExitCode);

public record SwitchStateChangedArgs(string Id, bool IsOn);
