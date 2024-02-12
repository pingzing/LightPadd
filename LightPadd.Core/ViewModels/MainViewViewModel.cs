using System;
using System.Linq;
using System.Timers;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightPadd.Core.Events;
using LightPadd.Core.Models.Options;
using LightPadd.Core.Services;
using Microsoft.Extensions.Options;

namespace LightPadd.Core.ViewModels;

public partial class MainViewViewModel : ViewModelBase
{
    private readonly ScreenIdleService _screenIdleService;
    private readonly IOptionsMonitor<HubitatOptions> _hubitatOptions;
    private readonly Timer _debounceTimer = new();

    [ObservableProperty]
    private IBrightnessService _brightnessService;

    public MainViewViewModel(
        IBrightnessService brightnessService,
        ScreenIdleService screenIdleService,
        IOptionsMonitor<HubitatOptions> hubitatOptions
    )
    {
        _brightnessService = brightnessService;
        _screenIdleService = screenIdleService;
        _hubitatOptions = hubitatOptions;
        _hubitatOptions.OnChange(
            (newOpts) =>
            {
                _debounceTimer.Debounce(
                    () =>
                    {
                        HubitatOptionsChanged(newOpts);
                    },
                    TimeSpan.FromSeconds(1)
                );
            }
        );

        Rooms.ResetBehavior = ResetBehavior.Remove; // Need to do this, or .Clear() won't totally clear things out.
        InitializeRoomList(_hubitatOptions.CurrentValue.Rooms);
    }

    private void HubitatOptionsChanged(HubitatOptions options)
    {
        InitializeRoomList(options.Rooms);
    }

    private void InitializeRoomList(HubitatRoom[] rooms)
    {
        foreach (RoomViewModel room in Rooms)
        {
            room.Dispose();
        }

        Rooms.Clear();
        var newRooms = rooms.Select(VMResolverService.Resolve<RoomViewModel, HubitatRoom>);
        Rooms.AddRange(newRooms);
        SelectedRoom = Rooms.FirstOrDefault();
    }

    [ObservableProperty]
    private AvaloniaList<RoomViewModel> _rooms = new();

    [ObservableProperty]
    private RoomViewModel? _selectedRoom = null;

    [RelayCommand]
    private void RoomTapped(RoomViewModel tappedRoomVm)
    {
        SelectedRoom = tappedRoomVm;
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
            new RoomViewModel() { Title = "SECOND ROOM" },
        ];
        SelectedRoom = Rooms.First();
    }
}
