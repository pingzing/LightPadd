using System.Diagnostics;

namespace LightPadd.Core.Services;

public class DummyBrightnessService : IBrightnessService
{
    private bool _dummyIsScreenOn = true;
    public bool IsScreenOn
    {
        get => _dummyIsScreenOn;
        set
        {
            Debug.WriteLine("Setting dummy IsScreenOn to " + value);
            _dummyIsScreenOn = value;
        }
    }

    private byte _dummyBrightness = 128;
    public byte Brightness
    {
        get => _dummyBrightness;
        set
        {
            Debug.WriteLine("Setting dummy Brightness to: " + value);
            _dummyBrightness = value;
        }
    }
}
