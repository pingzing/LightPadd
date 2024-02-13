namespace LightPadd.Core.DeviceBehaviors.Switches;

public abstract class PlugBehaviorBase
{
    public abstract string[] GetOnSequence();

    public abstract string[] GetOffSequence();
}
