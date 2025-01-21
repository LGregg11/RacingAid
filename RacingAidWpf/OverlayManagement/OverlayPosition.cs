namespace RacingAidWpf.OverlayManagement;

public class OverlayPosition(string name, Position position)
{
    public string Name { get; init; } = name;
    public Position Position { get; init; } = position;
}

public class Position(int top, int left)
{
    public int Top { get; init; } = top;
    public int Left { get; init; } = left;
}