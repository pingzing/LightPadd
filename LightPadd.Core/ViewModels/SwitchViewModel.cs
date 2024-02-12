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

public partial class SwitchViewModel : ViewModelBase, IDisposable
{
    private readonly HubitatClient _client;
    private readonly string _roomId;
    private readonly IMessenger _messenger;
    private readonly Device _backingDevice;

    private bool _isDisposed;

    public SwitchViewModel(IMessenger messenger, Device device, HubitatClient client, string roomId)
    {
        _client = client;
        _roomId = roomId;
        _messenger = messenger;
        _backingDevice = device;

        var switchAttr = _backingDevice.Attributes?.FirstOrDefault(x => x.Name == "switch");
        if (switchAttr is EnumDeviceAttribute enumAttr)
        {
            IsOn = enumAttr.CurrentValue == "on";
        }
        Name = _backingDevice.Label.ToUpperInvariant();

        // Listen for messages from the WebserverService that this switch's state has changed.
        _messenger.Register<SwitchStateChangedArgs, string>(
            this,
            $"{_roomId}-{_backingDevice.Id}",
            OnDeviceEvent
        );
    }

    [ObservableProperty]
    private bool _isOn = false;

    [ObservableProperty]
    private string _name;

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
    }

    private void OnDeviceEvent(object _, SwitchStateChangedArgs message) => IsOn = message.IsOn;

    // Designer-only.
    public SwitchViewModel()
    {
        _client = null!;
        _messenger = null!;
        _backingDevice = null!;
        _roomId = "n/a";
        Name = "ToggleButton".ToUpperInvariant();
    }

    protected virtual void Dispose(bool disposingManaged)
    {
        if (!_isDisposed)
        {
            if (disposingManaged)
            {
                _messenger.Unregister<SwitchStateChangedArgs, string>(
                    this,
                    $"{_roomId}-{_backingDevice.Id}"
                );
            }
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposingManaged)' method
        Dispose(disposingManaged: true);
        GC.SuppressFinalize(this);
    }
}
