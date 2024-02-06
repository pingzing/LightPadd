using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LightPadd.Core.Messaging;
using LightPadd.Core.Models.Hubitat;
using LightPadd.Core.Services;

namespace LightPadd.Core.ViewModels;

public partial class SwitchViewModel : ViewModelBase
{
    private readonly LivingRoomClient _client;
    private readonly IMessenger _messenger;
    private readonly Device _backingDevice;

    public SwitchViewModel(LivingRoomClient client, IMessenger messenger, Device device)
    {
        _client = client;
        _messenger = messenger;
        _backingDevice = device;

        var switchAttr = _backingDevice.Attributes?.FirstOrDefault(x => x.Name == "switch");
        if (switchAttr is EnumDeviceAttribute enumAttr)
        {
            IsOn = enumAttr.CurrentValue == "on";
        }
        Name = _backingDevice.Label.ToUpperInvariant();

        _messenger.Register<SwitchStateChangedArgs, string>(this, device.Id, OnDeviceEvent);
    }

    [ObservableProperty]
    private bool _isOn = false;

    [ObservableProperty]
    private string _name;

    // Only triggered by UI interaction, i.e. the user has requested a state change
    [RelayCommand]
    private async Task SwitchToggled(bool isChecked)
    {
        string command = isChecked ? "on" : "off";
        var response = await _client.SendCommand(_backingDevice.Id, command);
        if (response == null)
        {
            Console.WriteLine(
                $"Failed to update device {_backingDevice.Id} with command {command}"
            );
        }
        // Updating the ToggleButton's state will happen via the Hubitat POST-ing back
        // a response, which gets handled over in WebserverService, and sent here
        // then handled via OnDeviceEvent below.
    }

    private void OnDeviceEvent(object _, SwitchStateChangedArgs message) => IsOn = message.IsOn;

    // Designer-only.
    public SwitchViewModel()
    {
        _client = null!;
        _messenger = null!;
        _backingDevice = null!;
        Name = "ToggleButton".ToUpperInvariant();
    }
}
