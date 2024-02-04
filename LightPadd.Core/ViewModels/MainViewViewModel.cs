using System.Threading.Tasks;
using LightPadd.Core.Services;

namespace LightPadd.Core.ViewModels;

public partial class MainViewViewModel : ViewModelBase
{
    private readonly ScreenBrightnessService _brightnessService;
    private readonly ScreenIdleService _screenIdleService;

    public LivingRoomViewModel LivingRoomVM { get; init; }

    public MainViewViewModel(
        ScreenBrightnessService brightnessService,
        ScreenIdleService screenIdleService
    )
    {
        _brightnessService = brightnessService;
        _screenIdleService = screenIdleService;
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

    public void OnActivity()
    {
        _screenIdleService.ActivityDetected();
    }

    // For the designer
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MainViewViewModel()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    { }
}
