using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        string? name = data?.GetType().FullName!.Replace("ViewModel", "View");
        if (name == null)
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
#pragma warning disable IL2057 // Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.
        Type? type = Type.GetType(name);
#pragma warning restore IL2057 // Unrecognized value passed to the parameter of method. It's not possible to guarantee the availability of the target type.

        Control? control = ResolveControl(type);
        if (control == null)
        {
            return new TextBlock { Text = "Not Found: " + name };
        }

        return control;
    }

    private Control? ResolveControl(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? type
    )
    {
        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return null;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
