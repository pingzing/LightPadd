using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightPadd.Core.Models.Options;
using LightPadd.Core.Services;
using Microsoft.Extensions.Options;

namespace LightPadd.Core.ViewModels;

public partial class MainViewViewModel : ViewModelBase
{
    private readonly ScreenBrightnessService _brightnessService;
    private readonly ScreenIdleService _screenIdleService;
    private readonly IOptionsMonitor<HubitatOptions> _hubitatOptions;

    public MainViewViewModel(
        ScreenBrightnessService brightnessService,
        ScreenIdleService screenIdleService,
        IOptionsMonitor<HubitatOptions> hubitatOptions
    )
    {
        _brightnessService = brightnessService;
        _screenIdleService = screenIdleService;
        _hubitatOptions = hubitatOptions;
        _hubitatOptions.OnChange(HubitatOptionsChanged);

        InitializeRoomList(_hubitatOptions.CurrentValue.Rooms);
    }

    private void HubitatOptionsChanged(HubitatOptions options)
    {
        InitializeRoomList(options.Rooms);
    }

    private void InitializeRoomList(HubitatRoom[] rooms)
    {
        Rooms.Clear();
        Rooms.AddRange(rooms.Select(VMResolverService.Resolve<RoomViewModel, HubitatRoom>));
        SelectedRoom = Rooms.FirstOrDefault();
    }

    [ObservableProperty]
    private AvaloniaList<RoomViewModel> _rooms = new();

    [ObservableProperty]
    private RoomViewModel? _selectedRoom = null;

    [RelayCommand]
    private void RoomTapped(RoomViewModel tappedRoomVm)
    {
        // TODO: Some restyling?
        SelectedRoom = tappedRoomVm;
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
    {
        Rooms =
        [
            new RoomViewModel() { Title = "EXAMPLE ROOM" },
            new RoomViewModel() { Title = "SECOND ROOM" }
        ];
        SelectedRoom = Rooms.First();
    }
}
