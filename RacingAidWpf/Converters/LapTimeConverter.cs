using System.Globalization;
using System.Windows.Data;

namespace RacingAidWpf.Converters;

[ValueConversion(typeof(int), typeof(string))]
public class LapTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not (int timeMs and > 0))
            return string.Empty;

        TimeSpan time = TimeSpan.FromMilliseconds(timeMs);

        if (time.Hours > 0)
            return $"{time.Hours}:{time.Minutes:D2}:{time.Seconds:D2}";

        return time.Minutes > 0
            ? $"{time.Minutes}:{time.Seconds:D2}.{time.Milliseconds:D3}"
            : $"{time.Seconds}.{time.Milliseconds:D3}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}