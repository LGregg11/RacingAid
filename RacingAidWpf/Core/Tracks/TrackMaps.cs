namespace RacingAidWpf.Core.Tracks;

public class TrackMaps(List<TrackMap> maps)
{
    public List<TrackMap> Maps { get; set; } = maps;
}

public class TrackMap(string name, List<TrackMapPosition> positions)
{
    public string Name { get; set; } = name;

    public List<TrackMapPosition> Positions { get; set; } = positions;
}

public class TrackMapPosition(float lapDistance, float x, float y, float z)
{
    public float LapDistance { get; set; } = lapDistance;
    public float X { get; set; } = x;
    public float Y { get; set; } = y;
    public float Z { get; set; } = z;

    public override string ToString() => $"Track Pos - Distance: {LapDistance:F2}, Pos: {X:F1}, {Y:F1}, {Z:F1}";
}