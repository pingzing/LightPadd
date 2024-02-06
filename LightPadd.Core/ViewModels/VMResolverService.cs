using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LightPadd.Core.ViewModels;

public class VMResolverService
{
    private static Lazy<IServiceProvider> _serviceProvider =
        new(
            () =>
            {
                return (App.Current as App)!.Services;
            },
            isThreadSafe: true
        );

    public static TViewModel Resolve<TViewModel>()
        where TViewModel : ViewModelBase
    {
        var resolvedVm = _serviceProvider.Value.GetService<TViewModel>();
        if (resolvedVm == null)
        {
            throw new ArgumentOutOfRangeException(
                $"Unable to find any registered ViewModels of type"
                    + $"{typeof(TViewModel).FullName}."
            );
        }

        return resolvedVm;
    }

    public static TViewModel Resolve<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1
    >(TArg1? arg1)
        where TViewModel : ViewModelBase
    {
        return ResolveDynamicParams<TViewModel>([arg1!]);
    }

    public static TViewModel Resolve<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel,
        TArg1,
        TArg2
    >(TArg1? arg1, TArg2? arg2)
        where TViewModel : ViewModelBase
    {
        return ResolveDynamicParams<TViewModel>([arg1!, arg2!]);
    }

    private static TViewModel ResolveDynamicParams<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel
    >(object[] userArgs)
    {
        int userArgCount = userArgs.Length;
        Type vmType = typeof(TViewModel);
        ConstructorInfo[] constructors = vmType.GetConstructors();
        if (constructors.Length == 0)
        {
            throw new ArgumentException(
                $"The type {vmType.Name} does not have any public constructors."
            );
        }

        ConstructorInfo? firstMultiparamConstructor = constructors.FirstOrDefault(x =>
            x.GetParameters().Length > userArgCount
        );
        if (firstMultiparamConstructor == null)
        {
            throw new ArgumentException(
                $"The type {vmType.Name} does not have a public constructor with at least ${userArgCount + 1} parameters."
            );
        }

        ParameterInfo[] firstConstructorParameters = firstMultiparamConstructor.GetParameters();

        // Resolve dependencies for every parameter except for the last n--the user
        // should have passed those to us.
        List<object> constructorArgs = new();
        for (int i = 0; i < firstConstructorParameters.Length - userArgCount; i++)
        {
            ParameterInfo parameter = firstConstructorParameters[i];
            object? resolvedDependency = _serviceProvider.Value.GetService(parameter.ParameterType);
            if (resolvedDependency == null)
            {
                throw new ArgumentException(
                    $"Unable to resolve type {parameter.ParameterType} for parameter {parameter.Name} when constructing {vmType.Name}"
                );
            }
            constructorArgs.Add(resolvedDependency);
        }

        // Add the final, user-passed arg
        constructorArgs.AddRange(userArgs);

        return (TViewModel)firstMultiparamConstructor.Invoke([.. constructorArgs]);
    }
}
