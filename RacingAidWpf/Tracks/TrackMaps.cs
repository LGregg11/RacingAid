namespace RacingAidWpf.Tracks;

public class TrackMaps(List<TrackMap> maps)
{
    public List<TrackMap> Maps { get; set; } = maps;
}

public class TrackMap(string name, List<TrackMapPosition> positions)
{
    public string Name { get; set; } = name;

    public List<TrackMapPosition> Positions { get; set; } = positions;
}

public class TrackMapPosition(float x, float y)
{
    public float X { get; set; } = x;
    public float Y { get; set; } = y;
}