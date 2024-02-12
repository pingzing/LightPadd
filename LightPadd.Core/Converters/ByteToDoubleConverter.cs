using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightPadd.Core.Converters;

public class ByteToDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        byte byteVal = (byte)value!;
        return (double)byteVal;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        double doubleVal = (double)value!;
        return (byte)doubleVal;
    }
}
