using System.Diagnostics.CodeAnalysis;

namespace LightPadd.Core.Views;

/// <summary>
/// Contains attributes that direct the trimmer to retain Views
/// whose usage can't be statically analyzed.
/// <br/><br/>
/// Any Views that get instantiated indirectly (i.e. via ViewModel -> ViewLocator creation
/// as a result of data binding).
/// should be specified here so they don't get trimmed away.
/// </summary>
public class ViewTrimHinter
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(RoomView))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SwitchView))]
    public static void RegisterTrimHints() { }
}
