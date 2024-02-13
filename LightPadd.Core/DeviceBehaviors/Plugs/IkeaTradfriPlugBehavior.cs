namespace LightPadd.Core.DeviceBehaviors.Switches;

public class IkeaTradfriPlugBehavior : PlugBehaviorBase
{
    public override string[] GetOnSequence() => ["on", "refresh"];

    public override string[] GetOffSequence() => ["off", "refresh"];
}
