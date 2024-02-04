using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace LightPadd.Core.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
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
}
