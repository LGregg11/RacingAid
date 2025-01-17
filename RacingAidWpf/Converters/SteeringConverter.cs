using System.Globalization;
using System.Windows.Data;

namespace RacingAidWpf.Converters;

[ValueConversion(typeof(float), typeof(string))]
public class SteeringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not float degrees)
            return "N/A";

        var direction = degrees < 0 ? "R" : "L";
        var degreesAbs = MathF.Abs(degrees);

        return $"{degreesAbs:F1}{direction}";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}