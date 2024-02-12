namespace LightPadd.Core.Services;

public interface IBrightnessService
{
    bool IsScreenOn { get; set; }
    byte Brightness { get; set; }
}
