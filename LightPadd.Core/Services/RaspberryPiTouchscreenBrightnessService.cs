using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LightPadd.Core.Services;

public class RaspberryPiTouchscreenBrightnessService : ObservableObject, IBrightnessService
{
    private const string ControlFilesDirectory = "/sys/class/backlight/rpi_backlight";
    private const string BrightnessFileName = "brightness";
    private const string BacklightFileName = "bl_power";

    private static readonly string _brightnessFilePath = Path.Combine(
        ControlFilesDirectory,
        BrightnessFileName
    );
    private static readonly string _backlightFilePath = Path.Combine(
        ControlFilesDirectory,
        BacklightFileName
    );

    private FileSystemWatcher _brightnessWatcher;
    private FileSystemWatcher _backlightWatcher;

    public RaspberryPiTouchscreenBrightnessService()
    {
        // On application startup, ensure that the backlight is on, and that brightness is at least
        // enough to see, so that we don't get stuck in an uninteractable state.
        IsScreenOn = true;
        if (Brightness < 30)
        {
            Brightness = 30;
        }

        _brightnessWatcher = new FileSystemWatcher(ControlFilesDirectory, BrightnessFileName);
        _brightnessWatcher.EnableRaisingEvents = true;
        _brightnessWatcher.Changed += BrightnessWatcher_Changed;

        _backlightWatcher = new FileSystemWatcher(ControlFilesDirectory, BacklightFileName);
        _backlightWatcher.EnableRaisingEvents = true;
        _backlightWatcher.Changed += BacklightWatcher_Changed;
    }

    public bool IsScreenOn
    {
        get
        {
            string value = File.ReadAllText(_backlightFilePath);
            return value == "0";
        }
        set
        {
            // 0 means "on", 1 means "off"
            string onString = value ? "0" : "1";
            File.WriteAllText(_backlightFilePath, onString);
            OnPropertyChanged(nameof(IsScreenOn));
        }
    }

    public byte Brightness
    {
        get
        {
            string value = File.ReadAllText(_brightnessFilePath);
            return byte.Parse(value);
        }
        set
        {
            string byteString = value.ToString();
            File.WriteAllText(_brightnessFilePath, byteString);
            OnPropertyChanged(nameof(Brightness));
        }
    }

    private void BacklightWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        OnPropertyChanged(nameof(IsScreenOn));
    }

    private void BrightnessWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        OnPropertyChanged(nameof(Brightness));
    }
}
