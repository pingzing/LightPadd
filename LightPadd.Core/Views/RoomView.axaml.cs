using Avalonia.Controls;
using Avalonia.Interactivity;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core.Views;

public partial class RoomView : UserControl
{
    private RoomViewModel? _viewModel;

    public RoomView()
    {
        InitializeComponent();
    }

    private async void LivingRoom_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        _viewModel = (RoomViewModel)DataContext!;
        await _viewModel.Initialize();
    }
}
