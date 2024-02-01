using System;
using Microsoft.Extensions.DependencyInjection;

namespace LightPadd.Core.ViewModels;

public class VMResolver
{
    public static TViewModel Resolve<TViewModel>()
        where TViewModel : ViewModelBase
    {
        var resolvedVm = (App.Current as App)!.Services.GetService<TViewModel>();
        if (resolvedVm == null)
        {
            throw new ArgumentOutOfRangeException(
                $"Unable to find any registered ViewModels of type"
                    + $"{typeof(TViewModel).FullName}."
            );
        }

        return resolvedVm;
    }
}
