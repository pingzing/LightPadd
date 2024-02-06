using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
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
        var devices = await _hubitatClient.GetDevices();
        // TODO: Error handling/retrying/something
        if (devices != null)
        {
            var vms = devices.Select(VMResolverService.Resolve<SwitchViewModel, Device>);
            foreach (var vm in vms)
            {
                vm.PropertyChanged += LivingRoomVM_PropertyChanged;
            }
            Switches.AddRange(vms);
            _totalDevices = Switches.Count;
            DevicesOn = Switches.Where(x => x.IsOn).Count();
        }
    }

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

    private void LivingRoomVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(SwitchViewModel.IsOn))
        {
            return;
        }

        DevicesOn = Switches.Where(x => x.IsOn).Count();
    }

    // Designer-only, I guess
    public LivingRoomViewModel()
    {
        _brightnessService = null!;
        _hubitatClient = null!;
    }
}
