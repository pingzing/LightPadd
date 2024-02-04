using System.Diagnostics;
using Avalonia.Controls;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core;

public partial class MainWindow : Window
{
    private MainViewViewModel _mainViewModel;

    public MainWindow()
    {
        InitializeComponent();
        AddHandler(TappedEvent, MainWindow_Tapped, handledEventsToo: true);

        _mainViewModel = VMResolver.Resolve<MainViewViewModel>();
        MainView.DataContext = _mainViewModel;
    }

    private void MainWindow_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        // Route all tap inputs to the MainViewModel, so it can
        // inform the activity service to keep the screen awake,
        // or wake it up.
        Debug.WriteLine("MainWindow heard a tap.");
        _mainViewModel.OnActivity();
    }
}
