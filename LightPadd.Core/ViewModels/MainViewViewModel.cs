using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LightPadd.Core.Services;

namespace LightPadd.Core.ViewModels;

public partial class MainViewViewModel : ViewModelBase
{
    private readonly ScreenBrightnessService _brightnessService;

    public LivingRoomViewModel LivingRoomVM { get; init; }

    public MainViewViewModel(ScreenBrightnessService brightnessService)
    {
        _brightnessService = brightnessService;
        LivingRoomVM = VMResolver.Resolve<LivingRoomViewModel>();
    }

    public double Brightness
    {
        get => _brightnessService.Brightness;
        set
        {
            _brightnessService.Brightness = (byte)value;
            OnPropertyChanged();
        }
    }

    // For the designer
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MainViewViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    { }
}
