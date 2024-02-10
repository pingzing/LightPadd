using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using Avalonia;
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
using Microsoft.Extensions.Options;

namespace LightPadd.Core
{
    public partial class App : Application
    {
        private IMessenger _messenger = null!;
        private WebserverService _serverService = null!;
        public IServiceProvider Services { get; private set; } = null!;

        // Any Views that get instantiated indirectly (i.e. via ViewModel -> ViewLocator creation
        // as a result of data binding).
        // should be specified here so they don't get trimmed away.
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(RoomView))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SwitchView))]
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

        // Register types used by config stuff, so they don't get trimmed away.
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(HubitatOptions))]
        private ServiceProvider ConfigureServices()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            // Register configs
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            services.Configure<HubitatOptions>(config.GetSection(HubitatOptions.OptionsKey));
            HubitatOptions hubitatConfig = config
                .GetSection(HubitatOptions.OptionsKey)
                .Get<HubitatOptions>()!;
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

            // Services
            services.AddSingleton<ScreenBrightnessService>();
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
