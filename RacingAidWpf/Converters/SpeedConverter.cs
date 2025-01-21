using System.Globalization;
using System.Windows.Data;
using RacingAidWpf.Configuration;

namespace RacingAidWpf.Converters;

[ValueConversion(typeof(float), typeof(string))]
public class SpeedConverter : IValueConverter
{
    private const float MphFactor = 2.23694f;
    private const float KphFactor = 3.6f;
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not float speedMetresPerSecond)
            return "0";

        return ConfigSectionSingleton.TelemetrySection.UseMetricUnits
            ? $"{speedMetresPerSecond * KphFactor:F0} KPH"
            : $"{speedMetresPerSecond * MphFactor:F0} MPH";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}