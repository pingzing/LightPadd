using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Threading;
using LightPadd.Core.Services;

namespace LightPadd.Core;

public partial class MainView : UserControl
{
    private readonly HubitatClientService _hubitatClient;
    private readonly ScreenBrightnessService _brightnessService;
    private bool _isSquigglyLightOn = false;

    //temp stuff
    private IBrush _lcarsPeachBrush;
    private IBrush _dimPeachBrush;

    public MainView()
    {
        _hubitatClient = new HubitatClientService();
        _brightnessService = new ScreenBrightnessService();
        InitializeComponent();

        // TODO: Actually create LcarsToggleButton instead of this jank
        _lcarsPeachBrush = new SolidColorBrush((Color)Application.Current.Resources["LcarsPeach"]);
        _dimPeachBrush = new SolidColorBrush(
            (Color)Application.Current.Resources["LcarsPeach"],
            0.35
        );
        Console.WriteLine("Loaded.");
    }

    private async void MainView_Loaded(object? sender, RoutedEventArgs e)
    {
        // TODO: Dynamically assemble list of lights?
        var response = await _hubitatClient.GetDevicesDetailed("1");
        // TODO: Actually model all of this, holy shit
        var switchObject = response["attributes"]
            .AsArray()
            .FirstOrDefault(x => x["name"].GetValue<string>() == "switch");
        bool isOn = switchObject["currentValue"].GetValue<string>() == "on";
        _isSquigglyLightOn = isOn;
        if (_isSquigglyLightOn)
        {
            SquigglyLightButton.Background = _lcarsPeachBrush;
        }
        else
        {
            SquigglyLightButton.Background = _dimPeachBrush;
        }

        if (OperatingSystem.IsLinux())
        {
            BrightnessSlider.Value = _brightnessService.Brightness;
        }
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
            // The actual underlying status takes a few moments to update, so if
            // we get a 200, just assume that the operation worked
            // TODO: Maybe build this delay into the network service itself, so
            // consumers don't have to guess
            if (response != null)
            {
                _isSquigglyLightOn = false;
            }
        }
        else
        {
            var response = await _hubitatClient.SendCommand("1", "on");
            if (response != null)
            {
                _isSquigglyLightOn = true;
            }
        }

        Dispatcher.UIThread.Invoke(() =>
        {
            if (_isSquigglyLightOn)
            {
                SquigglyLightButton.Background = _lcarsPeachBrush;
            }
            else
            {
                SquigglyLightButton.Background = _dimPeachBrush;
            }
        });
    }

    private void BrightnessSlider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (OperatingSystem.IsLinux())
        {
            _brightnessService.Brightness = (byte)e.NewValue;
        }
        else
        {
            Debug.WriteLine($"DUMMY: Setting brightness to {(byte)e.NewValue}");
        }
    }
}
