using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DrawingColor = System.Drawing.Color;
using MediaColor = System.Windows.Media.Color;

namespace RacingAidWpf.Converters;

public class SafetyRatingColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is DrawingColor color
            ? new SolidColorBrush(MediaColor.FromArgb(color.A, color.R, color.G, color.B))
            : Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}