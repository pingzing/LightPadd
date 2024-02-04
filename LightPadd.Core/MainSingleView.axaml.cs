using Avalonia.Controls;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core;

public partial class MainSingleView : UserControl
{
    private MainViewViewModel _mainViewModel;

    public MainSingleView()
    {
        InitializeComponent();
        AddHandler(TappedEvent, MainSingleView_Tapped, handledEventsToo: true);

        _mainViewModel = VMResolver.Resolve<MainViewViewModel>();
        MainView.DataContext = _mainViewModel;
    }

    private void MainSingleView_Tapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        // Route all tap inputs to the MainViewModel, so it can
        // inform the activity service to keep the screen awake,
        // or wake it up.
        _mainViewModel.OnActivity();
    }
}
