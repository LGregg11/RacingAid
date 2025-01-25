namespace RacingAidWpf.OverlayManagement;

public class OverlayPosition(string name, ScreenPosition position)
{
    public string Name { get; } = name;
    public ScreenPosition Position { get; } = position;
}

public class ScreenPosition(double top, double left)
{
    public double Top { get; } = top;
    public double Left { get; } = left;
}