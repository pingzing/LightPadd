using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using LightPadd.Core.Models.Hubitat;
using LightPadd.Core.Models.Options;
using LightPadd.Core.Networking;
using LightPadd.Core.Services;
using Microsoft.Extensions.Options;

namespace LightPadd.Core.ViewModels;

public partial class RoomViewModel : ViewModelBase, IDisposable
{
    private readonly HubitatClient _client;
    private readonly HubitatOptions _options;
    private readonly NetworkOptions _networkOptions;
    private readonly HubitatRoom _backingRoom;

    private bool _isDisposed;

    public RoomViewModel(
        HubitatClientFactory clientFactory,
        IOptions<HubitatOptions> options,
        IOptions<NetworkOptions> networkOptions,
        HubitatRoom backingRoom
    )
    {
        _client = clientFactory.Create(backingRoom.Id, backingRoom.AccessToken);
        _options = options.Value;
        _networkOptions = networkOptions.Value;
        _backingRoom = backingRoom;

        Devices.ResetBehavior = ResetBehavior.Remove;
        Title = "LOADING...";
    }

    public async Task Initialize()
    {
        ClearDevices();
        Title = "LOADING...";

        bool postbackUrlSet = await SetPostbackUrl();
        Console.WriteLine($"Setting postback URL in Initiialize's result was: {postbackUrlSet}");

        var devices = await _client.GetDevices();
        if (devices == null)
        {
            Title = "ERROR!";
            return;
        }

        Title = devices.FirstOrDefault()?.Room.ToUpperInvariant() ?? "UNKNOWN ROOM";

        var viewModels = devices
            .Select(x =>
                VMResolverService.Resolve<SwitchViewModel, Device, HubitatClient, string>(
                    x,
                    _client,
                    _backingRoom.Id
                )
            )
            .ToList();

        foreach (var vm in viewModels)
        {
            vm.PropertyChanged += SwitchVM_PropertyChanged;
        }
        Devices.AddRange(viewModels);

        _totalDevices = Devices.Count;
        DevicesOn = Devices.Where(x => x.IsOn).Count();
    }

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private AvaloniaList<SwitchViewModel> _devices = new();

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

    private async Task<bool> SetPostbackUrl()
    {
        if (!_networkOptions.SetPostbackUrl)
        {
            return false;
        }

        // Get current IP address, this room's postback URL and port, and glue them all together
#if DEBUG
        var localWifiIPs = LocalAddresses.GetLocalIPv4Addresses();
#else
        var localWifiIPs = LocalAddresses.GetLocalIPv4Addresses();
#endif
        // Only get things that are actually local to THIS router.
        string networkPrefix = _networkOptions.NetworkPrefix;
        string? firstWifiIP = localWifiIPs.FirstOrDefault(x => x.StartsWith(networkPrefix));
        if (firstWifiIP == null)
        {
            Console.WriteLine($"Unable to find a valid local IP address. Candidates examined");
            foreach (string ip in localWifiIPs)
            {
                Console.Write(ip + ",");
            }
            Console.WriteLine();
            return false;
        }

        string postbackUrl =
            $"http://{firstWifiIP}:{_options.PostbackUrlPort}{_backingRoom.PostbackUrlPath}";
        Console.WriteLine($"Attempting to set postback url to {postbackUrl}");
        return await _client.SendPostbackUrl(postbackUrl);
    }

    private void SwitchVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(SwitchViewModel.IsOn))
        {
            return;
        }

        DevicesOn = Devices.Where(x => x.IsOn).Count();
    }

    private void ClearDevices()
    {
        foreach (var device in Devices)
        {
            device.Dispose();
        }
        Devices.Clear();
    }

    // Designer-only
    public RoomViewModel()
    {
        _backingRoom = null!;
        _client = null!;
        _options = null!;
        _networkOptions = null!;
        Title = "ROOM PLACEHOLDER TITLE";
    }

    protected virtual void Dispose(bool disposingManaged)
    {
        if (!_isDisposed)
        {
            if (disposingManaged)
            {
                ClearDevices();
            }

            Devices = null!;
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
