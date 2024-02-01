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
using LightPadd.Core.ViewModels;

namespace LightPadd.Core.Views;

public partial class MainView : UserControl
{
    private MainViewViewModel ViewModel;

    public MainView()
    {
        InitializeComponent();
        ViewModel = (MainViewViewModel)DataContext!;

        Console.WriteLine("Loaded.");
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
