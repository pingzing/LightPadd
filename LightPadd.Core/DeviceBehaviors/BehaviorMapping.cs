using System.Collections.Generic;
using LightPadd.Core.DeviceBehaviors.Switches;

namespace LightPadd.Core.DeviceBehaviors
{
    public static class BehaviorMapping
    {
        public const string Default = "Default";

        private static readonly DefaultPlugBehavior _defaultBehavior = new();
        private static readonly IkeaTradfriPlugBehavior _tradfriPlugBehavior = new();

        public static Dictionary<string, PlugBehaviorBase> PlugBehaviors { get; } =
            new()
            {
                { Default, _defaultBehavior },
                { "Generic Zigbee Outlet", _defaultBehavior },
                { "Ikea TRADFRI Control Outlet", _tradfriPlugBehavior }
            };
    }
}
