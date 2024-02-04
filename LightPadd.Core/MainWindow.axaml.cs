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
        _mainViewViewModel = VMResolver.Resolve<MainViewViewModel>();
        MainView.DataContext = _mainViewViewModel;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        _mainViewViewModel.OnActivity();
    }
}
