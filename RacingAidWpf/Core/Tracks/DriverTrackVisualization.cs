using System.Windows.Media;

namespace RacingAidWpf.Core.Tracks;

public class DriverTrackVisualization(double x, double y, double radius, int number, Brush fill, Brush border, double borderThickness)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
    public double Radius { get; set; } = radius;
    public int Number { get; set; } = number;
    public Brush Fill { get; set; } = fill;
    public Brush Border { get; set; } = border;
    public double BorderThickness { get; set; } = borderThickness;
}