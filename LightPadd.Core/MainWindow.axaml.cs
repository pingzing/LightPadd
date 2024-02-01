using Avalonia.Controls;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainView.DataContext = VMResolver.Resolve<MainViewViewModel>();
    }
}
