using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightPadd.Core.Services;

namespace LightPadd.Core.ViewModels;

public partial class LivingRoomViewModel : ViewModelBase
{
    private readonly ScreenBrightnessService _brightnessService;
    private readonly HubitatClient _hubitatClient;

    public LivingRoomViewModel(
        ScreenBrightnessService brightnessService,
        HubitatClient hubitatClient
    )
    {
        _brightnessService = brightnessService;
        _hubitatClient = hubitatClient;
    }

    public async Task Initialize()
    {
        // TODO: Dynamically assemble list of lights?
        var response = await _hubitatClient.GetDevicesDetailed("1");
        // TODO: Actually model all of this, holy shit
        var switchObject = response["attributes"]
            .AsArray()
            .FirstOrDefault(x => x["name"].GetValue<string>() == "switch");
        bool isOn = switchObject["currentValue"].GetValue<string>() == "on";

        _isSquigglyLightOn = isOn;
        OnPropertyChanged(nameof(IsSquigglyLightOn));
    }

    private bool _isSquigglyLightOn = false;
    public bool IsSquigglyLightOn
    {
        get => _isSquigglyLightOn;
        set
        {
            if (_isSquigglyLightOn == value)
            {
                return;
            }
            SquigglyLightPressed(value);
        }
    }

    private async void SquigglyLightPressed(bool newOnValue)
    {
        if (newOnValue)
        {
            var response = await _hubitatClient.SendCommand("1", "on");
            // The actual underlying status takes a few moments to update, so if
            // we get a 200, just assume that the operation worked
            // TODO: Maybe build this delay into the network service itself, so
            // consumers don't have to guess
            if (response != null)
            {
                _isSquigglyLightOn = true;
            }
        }
        else
        {
            var response = await _hubitatClient.SendCommand("1", "off");
            if (response != null)
            {
                _isSquigglyLightOn = false;
            }
        }

        OnPropertyChanged(nameof(IsSquigglyLightOn));
    }

    // Designer-only, I guess
    public LivingRoomViewModel()
    {
        _brightnessService = null!;
        _hubitatClient = null!;
    }
}
