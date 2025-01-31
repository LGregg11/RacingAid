using System.Windows;
using System.Windows.Media;

namespace RacingAidWpf.Tracks;

public static class TrackMapPathCreator
{
    public static List<TrackMapPosition> GetScaledTrackMapPositions(TrackMap trackMap, float factor)
    {
        var positions = trackMap.Positions;
        var minMaxValues = CalculateTrackMapMinMaxValues(positions);
        
        factor *= Calculate2DScalingFactor(minMaxValues);

        var x = minMaxValues.X;
        var y = minMaxValues.Y;
        var z = minMaxValues.Z;
        
        var xyRatio = x.Range / y.Range;
        var xFactor = xyRatio < 1 ? 1 : xyRatio;
        var yFactor = xyRatio > 1 ? 1 : xyRatio;

        var scaledPositions = new List<TrackMapPosition>();
        foreach (var position in positions)
        {
            var normalisedX = (position.X - x.Min) / (x.Range); 
            var normalisedY = (position.Y - y.Min) / (y.Range);
            var normalisedZ = (position.Z - z.Min) / (z.Range);

            scaledPositions.Add(
                new TrackMapPosition(
                    position.LapDistance,
                    normalisedX * xFactor * factor,
                    (y.Max - normalisedY) * yFactor * factor,
                    normalisedZ));
        }

        return scaledPositions;
    }
    
    public static GeometryGroup Create2DGeometryGroupFromTrackMap(TrackMap trackMap, float factor)
    {
        var geometryGroup = new GeometryGroup();

        var positions = trackMap.Positions;
        var nPositions = positions.Count;
        var minMaxValues = CalculateTrackMapMinMaxValues(positions);
        factor *= Calculate2DScalingFactor(minMaxValues);

        var yMax = minMaxValues.Y.Max;

        for (var i = 1; i < nPositions; i++)
        {
            var previousPosition = positions[i - 1];
            var currentPosition = positions[i];
            
            var startPoint = new Point(previousPosition.X * factor, (yMax - previousPosition.Y) * factor);
            var endPoint = new Point(currentPosition.X * factor, (yMax - currentPosition.Y) * factor);

            geometryGroup.Children.Add(new LineGeometry(startPoint, endPoint));
        }
        
        return geometryGroup;
    }

    private static float Calculate2DScalingFactor(MinMaxValues minMaxValues)
    {

        var xRange = minMaxValues.X.Max - minMaxValues.X.Min;
        var yRange = minMaxValues.Y.Max - minMaxValues.Y.Min;
        var max = yRange > xRange ? yRange : xRange;

        // 0.95 to act as 'padding'
        return 0.95f / max;
    }

    private static MinMaxValues CalculateTrackMapMinMaxValues(List<TrackMapPosition> positions)
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
}

internal class MinMaxValues(MinMaxValue x, MinMaxValue y, MinMaxValue z)
{
    public MinMaxValue X { get; set; } = x;
    public MinMaxValue Y { get; set; } = y;
    public MinMaxValue Z { get; set; } = z;
}

internal class MinMaxValue(float min, float max)
{
    public float Min { get; set; } = min;
    public float Max { get; set; } = max;
    public float Range => Max - Min;
}