using System.Globalization;
using System.Windows.Data;
using Accessibility;
using LiveCharts.Wpf;

namespace RacingAidWpf.Converters;

public enum RelativeLapState
{
    TwoBehind,
    OneBehind,
    SameLap,
    OneAhead,
    TwoAhead
}

public class LapBehindAheadBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not float floatValue)
            return RelativeLapState.SameLap;

        return floatValue switch
        {
            > 1.5f => RelativeLapState.TwoAhead,
            > 0.5f => RelativeLapState.OneAhead,
            < -1.5f => RelativeLapState.TwoBehind,
            < -0.5f => RelativeLapState.OneBehind,
            _ => RelativeLapState.SameLap
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}