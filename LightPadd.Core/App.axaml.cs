using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LightPadd.Core.Services;
using LightPadd.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LightPadd.Core
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; } = null!;

        public override void Initialize()
        {
            Services = ConfigureServices();

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
            services.AddHttpClient<HubitatClient>(x =>
            {
                x.BaseAddress = new Uri("http://192.168.0.44/apps/api/3/devices/");
            });
            services.AddSingleton<WebserverService>();

            // ViewModels
            services.AddTransient<MainViewViewModel>();
            services.AddTransient<LivingRoomViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
