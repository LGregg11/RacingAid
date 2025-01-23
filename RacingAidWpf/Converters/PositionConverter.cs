using System.Globalization;
using System.Windows.Data;

namespace RacingAidWpf.Converters;

[ValueConversion(typeof(int), typeof(string))]
public class PositionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int position || position == 0)
            return "-";

        return position.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}