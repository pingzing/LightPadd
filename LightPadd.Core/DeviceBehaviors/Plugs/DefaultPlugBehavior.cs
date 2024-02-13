namespace LightPadd.Core.DeviceBehaviors.Switches;

public class DefaultPlugBehavior : PlugBehaviorBase
{
    public override string[] GetOnSequence() => ["on"];

    public override string[] GetOffSequence() => ["off"];
}
