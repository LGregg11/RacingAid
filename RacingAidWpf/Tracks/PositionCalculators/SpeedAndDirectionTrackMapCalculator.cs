using RacingAidData.Core.Models;

namespace RacingAidWpf.Tracks.PositionCalculators;

public class SpeedAndDirectionTrackMapCalculator : TrackMapPositionCalculator
{
    public override TrackMapPosition CalculatePosition(TrackMapPosition previousPosition, DriverDataModel driverDataModel)
    {
        // Use euler-esque approximations to determine the driver's current whereabouts..
        var approximateNewX = previousPosition.X + driverDataModel.SpeedMs * MathF.Sin(driverDataModel.ForwardDirectionDeg);
        var approximateNewY = previousPosition.Y + driverDataModel.SpeedMs * MathF.Cos(driverDataModel.ForwardDirectionDeg);

        return new TrackMapPosition(approximateNewX, approximateNewY);
    }
}