using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LightPadd.Core.Services;

namespace LightPadd.Core;

public partial class MainView : UserControl
{
    private readonly HubitatClientService _hubitatClient;
    private bool _isSquigglyLightOn = false;

    public MainView()
    {
        _hubitatClient = new HubitatClientService();
        InitializeComponent();
    }

    private async void MainView_Loaded(object? sender, RoutedEventArgs e)
    {
        var response = await _hubitatClient.GetDevicesDetailed("1");
        var switchObject = response["attributes"]
            .AsArray()
            .FirstOrDefault(x => x["name"].GetValue<string>() == "switch");
        bool isOn = switchObject["currentValue"].GetValue<string>() == "on";
        _isSquigglyLightOn = isOn;
    }

    public void Button_Click(object? sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }

    private void FlyoutRestart_Click(object? sender, RoutedEventArgs e)
    {
        if (OperatingSystem.IsWindows())
        {
            Debug.WriteLine("Restart button pressed. Ignoring, on dev machine.");
        }

        if (OperatingSystem.IsLinux())
        {
            Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "reboot" });
        }
    }

    private void FlyoutShutodwn_Click(object? sender, RoutedEventArgs e)
    {
        if (OperatingSystem.IsWindows())
        {
            Debug.WriteLine("Shutdown button pressed. Ignoring, on dev machine.");
        }

        if (OperatingSystem.IsLinux())
        {
            Process.Start(new ProcessStartInfo() { FileName = "sudo", Arguments = "shutdown now" });
        }
    }

    private async void SquigglyLightButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_isSquigglyLightOn)
        {
            var response = await _hubitatClient.SendCommand("1", "off");
            var switchObject = response["attributes"]
                .AsArray()
                .FirstOrDefault(x => x["name"].GetValue<string>() == "switch");
            bool isOn = switchObject["currentValue"].GetValue<string>() == "on";
            _isSquigglyLightOn = isOn;
        }
        else
        {
            var response = await _hubitatClient.SendCommand("1", "on");
            var switchObject = response["attributes"]
                .AsArray()
                .FirstOrDefault(x => x["name"].GetValue<string>() == "switch");
            bool isOn = switchObject["currentValue"].GetValue<string>() == "on";
            _isSquigglyLightOn = isOn;
        }
    }
}
