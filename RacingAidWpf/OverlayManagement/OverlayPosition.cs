namespace RacingAidWpf.OverlayManagement;

public class OverlayPosition(string name, Position position)
{
    public string Name { get; } = name;
    public Position Position { get; } = position;
}

public class Position(double top, double left)
{
    public double Top { get; } = top;
    public double Left { get; } = left;
}