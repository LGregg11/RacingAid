namespace RacingAidWpf.Tracks;

public static class TrackMapUtilities
{
    public static List<TrackMapPosition> UpdatePositions(List<TrackMapPosition> positions, bool normalize, bool invertY, float scaleFactor = 0f)
    {
        var minMaxValues = CalculateTrackMapMinMaxValues(positions);
        
        var updatedPositions = new List<TrackMapPosition>();
        foreach (var position in positions)
        {
            var updatedPosition = position;
            if (invertY)
                updatedPosition = InvertYPosition(updatedPosition);
            
            if (normalize)
                updatedPosition = NormalizePosition(updatedPosition, minMaxValues);

            if (scaleFactor != 0f)
                updatedPosition = ScalePosition(updatedPosition, scaleFactor);
            
            updatedPositions.Add(updatedPosition);
        }
        
        return updatedPositions;
    }
    
    public static MinMaxValues CalculateTrackMapMinMaxValues(List<TrackMapPosition> positions)
    {
        var xMin = float.MaxValue;
        var xMax = float.MinValue;
        var yMin = float.MaxValue;
        var yMax = float.MinValue;
        var zMin = float.MaxValue;
        var zMax = float.MinValue;
        
        foreach (var position in positions)
        {
            if (position.X < xMin)
                xMin = position.X;
            if (position.X > xMax)
                xMax = position.X;
            
            if (position.Y < yMin)
                yMin = position.Y;
            if (position.Y > yMax)
                yMax = position.Y;
            
            if (position.Z < zMin)
                zMin = position.Z;
            if (position.Z > zMax)
                zMax = position.Z;
        }

        return new MinMaxValues(
            new MinMaxValue(xMin, xMax),
            new MinMaxValue(yMin, yMax),
            new MinMaxValue(zMin, zMax));
    }

    private static TrackMapPosition NormalizePosition(TrackMapPosition position, MinMaxValues minMaxValues)
    {
        var maxRange = minMaxValues.MaxRange;

        position.X = (position.X - minMaxValues.X.Min) / maxRange;
        position.Y = (position.Y - minMaxValues.Y.Min) / maxRange;
        position.Z = (position.Z - minMaxValues.Z.Min) / maxRange;
        return position;
    }

    private static TrackMapPosition InvertYPosition(TrackMapPosition position)
    {
        position.Y = -position.Y;
        return position;
    }

    private static TrackMapPosition ScalePosition(TrackMapPosition position, float scaleFactor)
    {
        position.X *= scaleFactor;
        position.Y *= scaleFactor;
        position.Z *= scaleFactor;
        return position;
    }
}

public class MinMaxValues(MinMaxValue x, MinMaxValue y, MinMaxValue z)
{
    public MinMaxValue X { get; set; } = x;
    public MinMaxValue Y { get; set; } = y;
    public MinMaxValue Z { get; set; } = z;

    public float MaxRange => new[] { X.Range, Y.Range, Z.Range }.Max();
}

public class MinMaxValue(float min, float max)
{
    public float Min { get; set; } = min;
    public float Max { get; set; } = max;
    public float Range => Max - Min;
}