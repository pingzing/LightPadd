using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace LightPadd.Core;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
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
            Process.Start(new ProcessStartInfo()
            {
                FileName = "sudo",
                Arguments = "reboot"
            });
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
            Process.Start(new ProcessStartInfo()
            {
                FileName = "sudo",
                Arguments = "shutdown now"
            });
        }
    }
}
