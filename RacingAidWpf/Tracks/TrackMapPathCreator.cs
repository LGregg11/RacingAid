using System.Windows;
using System.Windows.Media;

namespace RacingAidWpf.Tracks;

public static class TrackMapPathCreator
{
    public static GeometryGroup Create2DGeometryGroupFromTrackMap(List<TrackMapPosition> positions)
    {
        var geometryGroup = new GeometryGroup();

        var nPositions = positions.Count;

        for (var i = 1; i < nPositions; i++)
        {
            var previousPosition = positions[i - 1];
            var currentPosition = positions[i];

            var startPoint = new Point(previousPosition.X, previousPosition.Y);
            var endPoint = new Point(currentPosition.X, currentPosition.Y);

            geometryGroup.Children.Add(new LineGeometry(startPoint, endPoint));
        }
        
        return geometryGroup;
    }
}