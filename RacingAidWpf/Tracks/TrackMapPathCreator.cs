using System.Windows;
using System.Windows.Media;

namespace RacingAidWpf.Tracks;

public static class TrackMapPathCreator
{
    public static GeometryGroup CreateGeometryGroupFromTrackMap(TrackMap trackMap, float factor)
    {
        var geometryGroup = new GeometryGroup();

        var positions = trackMap.Positions;
        var nPositions = positions.Count;

        var yRange = positions.Max(p => p.Y) - positions.Min(p => p.Y);
        var xRange = positions.Max(p => p.X) - - positions.Min(p => p.X);
        
        factor = yRange > xRange ? factor / yRange : factor / xRange;
        
        // apply padding
        factor *= 0.95f;

        for (var i = 1; i < nPositions; i++)
        {
            var previousPosition = positions[i - 1];
            var currentPosition = positions[i];
            
            var startPoint = new Point(previousPosition.X * factor, (yRange - previousPosition.Y) * factor);
            var endPoint = new Point(currentPosition.X * factor, (yRange - currentPosition.Y) * factor);

            geometryGroup.Children.Add(new LineGeometry(startPoint, endPoint));
        }
        
        return geometryGroup;
    }
}