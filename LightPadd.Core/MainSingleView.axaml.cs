using Avalonia.Controls;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core;

public partial class MainSingleView : UserControl
{
    public MainSingleView()
    {
        InitializeComponent();
        MainView.DataContext = VMResolver.Resolve<MainViewViewModel>();
    }
}
