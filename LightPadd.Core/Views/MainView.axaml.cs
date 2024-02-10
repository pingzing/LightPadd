using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core.Views;

public partial class MainView : UserControl
{
    private MainViewViewModel? _viewModel;

    public MainView()
    {
        InitializeComponent();
        Loaded += MainView_Loaded;
    }

    private void MainView_Loaded(object? sender, RoutedEventArgs e)
    {
        _viewModel = (MainViewViewModel)DataContext!;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        // Do initial selection, because SelectedRoom might get set before our handler is hooked up
        if (_viewModel?.SelectedRoom != null)
        {
            UpdateButtonSelection(_viewModel.SelectedRoom);
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (
            e.PropertyName == nameof(MainViewViewModel.SelectedRoom)
            && _viewModel?.SelectedRoom != null
        )
        {
            UpdateButtonSelection(_viewModel.SelectedRoom);
        }
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

    private void UpdateButtonSelection(RoomViewModel selectedRoomVm)
    {
        // Handles the visual "selection" updating of the room buttons
        // Emulates radio button behavior.
        // Slow, but it works.
        List<Button> possibleButtons = RoomButtonStrip
            .GetLogicalDescendants()
            .Where(x => x is Button)
            .Cast<Button>()
            .ToList();

        var selectedButton = possibleButtons.FirstOrDefault(x => x.DataContext == selectedRoomVm);
        foreach (var button in possibleButtons)
        {
            button.Classes.Remove("selected");
        }
        selectedButton?.Classes.Add("selected");
    }
}
