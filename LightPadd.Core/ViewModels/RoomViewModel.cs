using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using LightPadd.Core.Models.Hubitat;
using LightPadd.Core.Models.Options;
using LightPadd.Core.Services;

namespace LightPadd.Core.ViewModels;

public partial class RoomViewModel : ViewModelBase
{
    private readonly HubitatRoom _backingRoom;
    private readonly HubitatClient _client;

    public RoomViewModel(HubitatClientFactory clientFactory, HubitatRoom backingRoom)
    {
        _client = clientFactory.Create(backingRoom.Id, backingRoom.AccessToken);
        _backingRoom = backingRoom;
        Title = "LOADING...";
    }

    public async Task Initialize()
    {
        var devices = await _client.GetDevices();
        if (devices == null)
        {
            Title = "ERROR!";
            return;
        }

        Title = devices.FirstOrDefault()?.Room.ToUpperInvariant() ?? "UNKNOWN ROOM";

        var vms = devices
            .Select(x =>
                VMResolverService.Resolve<SwitchViewModel, Device, HubitatClient, string>(
                    x,
                    _client,
                    _backingRoom.Id
                )
            )
            .ToList();
        foreach (var vm in vms)
        {
            vm.PropertyChanged += SwitchVM_PropertyChanged;
        }
        Switches.AddRange(vms);
        _totalDevices = Switches.Count;
        DevicesOn = Switches.Where(x => x.IsOn).Count();
    }

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private AvaloniaList<SwitchViewModel> _switches = new();

    private int _totalDevices = 0;
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

    private void SwitchVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(SwitchViewModel.IsOn))
        {
            return;
        }

        DevicesOn = Switches.Where(x => x.IsOn).Count();
    }

    // Designer-only
    public RoomViewModel()
    {
        _backingRoom = null!;
        _client = null!;
        Title = "ROOM PLACEHOLDER TITLE";
    }
}
