using Avalonia.Controls;
using Avalonia.Input;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core;

public partial class MainSingleView : UserControl
{
    private MainViewViewModel _mainViewViewModel;

    public MainSingleView()
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
