using System.IO;

namespace LightPadd.Core.Services;

public class ScreenBrightnessService
{
    private const string BrightnessFile = "/sys/class/backlight/rpi_backlight/brightness";
    private const string BacklightFile = "/sys/class/backlight/rpi_backlight/bl_power";

    // TODO: Set up file watchers so that if the value changes externally, we can make our slider update
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

    public byte Brightness
    {
        get
        {
            string value = File.ReadAllText(BrightnessFile);
            return byte.Parse(value);
        }
        set
        {
            string byteString = value.ToString();
            File.WriteAllText(BrightnessFile, byteString);
        }
    }
}
