using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using LightPadd.Core.Messaging;
using LightPadd.Core.Models.Options;
using LightPadd.Core.Services;
using LightPadd.Core.ViewModels;
using LightPadd.Core.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LightPadd.Core
{
    public partial class App : Application
    {
        private IMessenger _messenger = null!;
        private WebserverService _serverService = null!;
        public IServiceProvider Services { get; private set; } = null!;

        public override void Initialize()
        {
            if (Design.IsDesignMode)
            {
                // TODO: make this less dumb?
                AvaloniaXamlLoader.Load(this);
                return;
            }
            ViewTrimHinter.RegisterTrimHints();
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

        private ServiceProvider ConfigureServices()
        {
            OptionsTrimHinter.RegisterTrimHints();

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
                .AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true)
#endif
                .Build();

            var services = new ServiceCollection();

            // Register configs
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            services.Configure<HubitatOptions>(config.GetSection(HubitatOptions.OptionsKey));
            services.Configure<NetworkOptions>(config.GetSection(NetworkOptions.OptionsKey));
            HubitatOptions hubitatConfig = config
                .GetSection(HubitatOptions.OptionsKey)
                .Get<HubitatOptions>()!;
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

            // Services
            if (OperatingSystem.IsLinux())
            {
                services.AddSingleton<
                    IBrightnessService,
                    RaspberryPiTouchscreenBrightnessService
                >();
            }
            else
            {
                services.AddSingleton<IBrightnessService, DummyBrightnessService>();
            }
            services.AddSingleton<ScreenIdleService>();
            foreach (var room in hubitatConfig.Rooms)
            {
                services.AddHttpClient(
                    room.Id,
                    (httpClient) =>
                    {
                        httpClient.BaseAddress = new Uri(hubitatConfig.BaseUrl);
                    }
                );
            }
            services.AddSingleton<HubitatClientFactory>();
            services.AddSingleton<WebserverService>();
            services.AddSingleton<IMessenger, StrongReferenceMessenger>(sp =>
                StrongReferenceMessenger.Default
            );

            // ViewModels
            services.AddTransient<MainViewViewModel>();
            services.AddTransient<RoomViewModel>();
            services.AddTransient<SwitchViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
