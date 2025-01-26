using RacingAidData.Core.Models;

namespace RacingAidWpf.Tracks.PositionCalculators;

public abstract class TrackMapPositionCalculator
{
    public abstract TrackMapPosition CalculatePosition(TrackMapPosition previousPosition, DriverDataModel driverDataModel, float timeDeltaMs);
}