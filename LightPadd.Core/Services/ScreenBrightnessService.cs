using System;
using System.IO;

namespace LightPadd.Core.Services;

public class ScreenBrightnessService
{
    private const string BrightnessFile = "/sys/class/backlight/rpi_backlight/brightness";
    private const string BacklightFile = "/sys/class/backlight/rpi_backlight/bl_power";

    // TODO: Set up file watchers so that if the value changes externally, we can make our slider update
    // TODO: Set up screen timeout that sends a message over to some overlay touch control that sits on top of the MainView
    // and a) wakes up the screen and b) dismisses itself, on touch
    public ScreenBrightnessService() { }

    public bool IsScreenOn
    {
        get
        {
            string value = File.ReadAllText(BacklightFile);
            return value == "0";
        }
        set
        {
            // 0 means "on", 1 means "off"
            string onString = value ? "0" : "1";
            File.WriteAllText(BacklightFile, onString);
        }
    }

    private byte _dummyBrightness = 128;
    public byte Brightness
    {
        get
        {
            if (OperatingSystem.IsLinux())
            {
                string value = File.ReadAllText(BrightnessFile);
                return byte.Parse(value);
            }
            else
            {
                return _dummyBrightness;
            }
        }
        set
        {
            if (OperatingSystem.IsLinux())
            {
                string byteString = value.ToString();
                File.WriteAllText(BrightnessFile, byteString);
            }
            else
            {
                _dummyBrightness = value;
            }
        }
    }
}
