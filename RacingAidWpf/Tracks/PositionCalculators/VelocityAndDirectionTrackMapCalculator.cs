using RacingAidData.Core.Models;

namespace RacingAidWpf.Tracks.PositionCalculators;

public class SpeedAndDirectionTrackMapCalculator : TrackMapPositionCalculator
{
    private const float DegToRad = MathF.PI / 180f;
    private const float OneMetreInMm = 1000f;
    
    public override TrackMapPosition CalculatePosition(TrackMapPosition previousPosition, DriverDataModel driverDataModel, float timeDeltaMs)
    {
        // Use euler-esque approximations to determine the driver's current whereabouts..
        var approxDistanceMetres = timeDeltaMs * driverDataModel.SpeedMs / OneMetreInMm;
        
        var directionRad = driverDataModel.ForwardDirectionDeg * DegToRad;
        var approximateNewX = previousPosition.X + approxDistanceMetres * MathF.Sin(directionRad);
        var approximateNewY = previousPosition.Y + approxDistanceMetres * MathF.Cos(directionRad);
        return new TrackMapPosition(approximateNewX, approximateNewY);
    }
}