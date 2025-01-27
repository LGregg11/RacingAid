using System.Windows;
using System.Windows.Media;

namespace RacingAidWpf.Tracks;

public static class TrackMapPathCreator
{
    public static List<TrackMapPosition> GetScaledTrackMapPositions(TrackMap trackMap, float factor)
    {
        var positions = trackMap.Positions;
        
        factor *= CalculateScalingFactor(trackMap);

        var yMax = positions.Max(p => p.Y);

        return positions
            .Select(position => new TrackMapPosition(position.X * factor, (yMax - position.Y) * factor))
            .ToList();
    }
    
    public static GeometryGroup CreateGeometryGroupFromTrackMap(TrackMap trackMap, float factor)
    {
        var geometryGroup = new GeometryGroup();

        var positions = trackMap.Positions;
        var nPositions = positions.Count;

        var yMax = positions.Max(p => p.Y);

        factor *= CalculateScalingFactor(trackMap);

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

    private static float CalculateScalingFactor(TrackMap trackMap)
    {
        var positions = trackMap.Positions;

        var yMax = 0f;
        var yMin = 0f;
        var xMax = 0f;
        var xMin = 0f;
        
        foreach (var position in positions)
        {
            if (position.Y > yMax)
                yMax = position.Y;
            
            if (position.Y < yMin)
                yMin = position.Y;
            
            if (position.X > xMax)
                xMax = position.X;
            
            if (position.X < xMin)
                xMin = position.X;
        }

        var yRange = yMax - yMin;
        var xRange = xMax - xMin;
        var max = yRange > xRange ? yRange : xRange;

        // 0.95 to act as 'padding'
        return 0.95f / max;
    }
}