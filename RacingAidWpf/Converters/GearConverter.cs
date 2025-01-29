using System.Globalization;
using System.Windows.Data;

namespace RacingAidWpf.Converters;

[ValueConversion(typeof(int), typeof(string))]
public class GearConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int gear)
            return "N/A";

        return gear switch
        {
            -1 => "R",
            0 => "N",
            _ => gear.ToString()
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}