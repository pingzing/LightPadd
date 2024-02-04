using Avalonia.Controls;
using Avalonia.Interactivity;
using LightPadd.Core.ViewModels;

namespace LightPadd.Core.Views;

public partial class LivingRoomView : UserControl
{
    private LivingRoomViewModel? ViewModel;

    public LivingRoomView()
    {
        InitializeComponent();
    }

    private async void LivingRoom_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Design.IsDesignMode)
        {
            return;
        }
        ViewModel = (LivingRoomViewModel)DataContext!;
        await ViewModel.Initialize();
    }
}
