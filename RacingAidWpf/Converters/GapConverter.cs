using System.Globalization;
using System.Windows.Data;

namespace RacingAidWpf.Converters;

[ValueConversion(typeof(int), typeof(string))]
public class GapConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        const int oneTenthMs = 100;
        
        if (value is not int timeMs || Math.Abs(timeMs) < oneTenthMs)
            return "-";
        
        var signStr = timeMs > 0 ? "-" : "+";
        timeMs = Math.Abs(timeMs);
        
        var time = TimeSpan.FromMilliseconds(timeMs);
        var timeStr = time.Minutes > 0
            ? $"{time.Minutes}:{time.Seconds:D2}"
            : $"{time.Seconds}.{time.Milliseconds / 100}";
        
        return $"{signStr}{timeStr}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}