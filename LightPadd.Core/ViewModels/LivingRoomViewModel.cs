using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using LightPadd.Core.Models.Hubitat;
using LightPadd.Core.Services;

namespace LightPadd.Core.ViewModels;

public partial class LivingRoomViewModel : ViewModelBase
{
    private readonly ScreenBrightnessService _brightnessService;
    private readonly LivingRoomClient _hubitatClient;

    public LivingRoomViewModel(
        ScreenBrightnessService brightnessService,
        LivingRoomClient hubitatClient
    )
    {
        _brightnessService = brightnessService;
        _hubitatClient = hubitatClient;
    }

    public async Task Initialize()
    {
        // TODO: Dynamically assemble list of lights to add?
        // Definitely TODO: Create ButtonViewModel so lights can encapsulate all of this state
        var squigglyResponse = await _hubitatClient.GetDevice("1");
        var squigglySwitchAttr = squigglyResponse?.Attributes?.FirstOrDefault(x =>
            x.Name == "switch"
        );
        if (squigglySwitchAttr is EnumDeviceAttribute enumAttr)
        {
            bool isOn = enumAttr.CurrentValue == "on";
            _isSquigglyLightOn = isOn;
            if (_isSquigglyLightOn)
            {
                DevicesOn += 1;
            }
            OnPropertyChanged(nameof(IsSquigglyLightOn));
        }

        var bookcaseLedResponse = await _hubitatClient.GetDevice("2");
        var bookcaseSwitchAttr = bookcaseLedResponse?.Attributes?.FirstOrDefault(x =>
            x.Name == "switch"
        );
        if (bookcaseSwitchAttr is EnumDeviceAttribute bookcaseEnumAttr)
        {
            bool isOn = bookcaseEnumAttr.CurrentValue == "on";
            _isBookcaseLedOn = isOn;
            if (_isBookcaseLedOn)
            {
                DevicesOn += 1;
            }
            OnPropertyChanged(nameof(IsBookcaseLedOn));
        }
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
            // TODO: Instead of updating this based on the HTTP response (which can send old state),
            // instead let the WebserverService receive a POST event, and let that fire an event which we
            // capture, and use that to update state.
            if (response != null)
            {
                DevicesOn += 1;
                _isSquigglyLightOn = true;
            }
        }
        else
        {
            var response = await _hubitatClient.SendCommand("1", "off");
            if (response != null)
            {
                DevicesOn -= 1;
                _isSquigglyLightOn = false;
            }
        }

        OnPropertyChanged(nameof(IsSquigglyLightOn));
    }

    private bool _isBookcaseLedOn = false;
    public bool IsBookcaseLedOn
    {
        get => _isBookcaseLedOn;
        set
        {
            if (_isBookcaseLedOn == value)
            {
                return;
            }
            BookcaseLedPressed(value);
        }
    }

    private async void BookcaseLedPressed(bool newOnValue)
    {
        if (newOnValue)
        {
            var response = await _hubitatClient.SendCommand("2", "on");
            if (response != null)
            {
                DevicesOn += 1;
                _isBookcaseLedOn = true;
            }
        }
        else
        {
            var response = await _hubitatClient.SendCommand("2", "off");
            if (response != null)
            {
                DevicesOn -= 1;
                _isBookcaseLedOn = false;
            }
        }

        OnPropertyChanged(nameof(IsBookcaseLedOn));
    }

    private int _totalDevices = 2;

    private int _devicesOn = 0;
    public int DevicesOn
    {
        get => _devicesOn;
        set
        {
            _devicesOn = value;
            OnPropertyChanged(nameof(FooterText));
        }
    }

    public string FooterText => $"{DevicesOn} / {_totalDevices}";

    // Designer-only, I guess
    public LivingRoomViewModel()
    {
        _brightnessService = null!;
        _hubitatClient = null!;
    }
}
