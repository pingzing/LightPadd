using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LightPadd.Core.Services;
using LightPadd.Core.ViewModels;
using LightPadd.Core.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LightPadd.Core
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; } = null!;
        public WebserverService _serverService { get; private set; } = null!;

        // Any Views that get instantiated indirectly (i.e. via ViewModel -> ViewLocator creation
        // as a result of data binding).
        // should be specified here so they don't get trimmed away.
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(LivingRoomView))]
        public override void Initialize()
        {
            Services = ConfigureServices();
            _serverService = Services.GetRequiredService<WebserverService>();

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Remove Avalonia data validation.
            // Otherwise, we'll get duplicate validations from both Avalonia and the Community Toolkit.
            BindingPlugins.DataValidators.RemoveAt(0);
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                singleView.MainView = new MainSingleView();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<ScreenBrightnessService>();
            services.AddSingleton<ScreenIdleService>();
            services.AddHttpClient<LivingRoomClient>(
                (x) => x.BaseAddress = new Uri("http://192.168.0.44/apps/api/3/devices/")
            );
            services.AddSingleton<WebserverService>();

            // ViewModels
            services.AddTransient<MainViewViewModel>();
            services.AddTransient<LivingRoomViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
