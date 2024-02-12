using System.Diagnostics.CodeAnalysis;

namespace LightPadd.Core.Models.Options;

/// <summary>
/// Contains attributes that direct the trimmer to retain Options-related classes
/// whose usage can't be statically analyzed.
///
/// When adding new Options classes, make sure to add those new classes here.
/// </summary>
public class OptionsTrimHinter
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HubitatOptions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HubitatRoom))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(NetworkOptions))]
    public static void RegisterTrimHints() { }
}
