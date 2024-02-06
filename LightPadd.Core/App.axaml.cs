using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using LightPadd.Core.Messaging;
using LightPadd.Core.Services;
using LightPadd.Core.ViewModels;
using LightPadd.Core.Views;
using Microsoft.Extensions.DependencyInjection;

namespace LightPadd.Core
{
    public partial class App : Application
    {
        private IMessenger _messenger = null!;
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
            _messenger = Services.GetRequiredService<IMessenger>();

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

            if (ApplicationLifetime is IControlledApplicationLifetime controlledLifetime)
            {
                // Applies for both IClassicDesktopStyleApplicationLifetime and ISingleViewApplicationLifetime.
                controlledLifetime.Exit += OnExit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            _messenger.Send<AppShutdownMessage>(new(e.ApplicationExitCode));
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
            services.AddSingleton<IMessenger, StrongReferenceMessenger>(sp =>
                StrongReferenceMessenger.Default
            );

            // ViewModels
            services.AddTransient<MainViewViewModel>();
            services.AddTransient<LivingRoomViewModel>();
            services.AddTransient<SwitchViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
