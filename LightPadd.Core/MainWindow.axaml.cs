using Avalonia.Controls;
using Avalonia.Input;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core;

public partial class MainWindow : Window
{
    private MainViewViewModel _mainViewViewModel;

    public MainWindow()
    {
        InitializeComponent();
        PointerMovedEvent.AddClassHandler<TopLevel>(OnGlobalPointerMoved, handledEventsToo: true);
        PointerPressedEvent.AddClassHandler<TopLevel>(
            OnGlobalPointerPressed,
            handledEventsToo: true
        );
        _mainViewViewModel = VMResolver.Resolve<MainViewViewModel>();
        MainView.DataContext = _mainViewViewModel;
    }

    private void OnGlobalPointerMoved(TopLevel level, PointerEventArgs args)
    {
        _mainViewViewModel.OnActivity();
    }

    private void OnGlobalPointerPressed(TopLevel level, PointerPressedEventArgs args)
    {
        _mainViewViewModel.OnActivity();
    }
}
