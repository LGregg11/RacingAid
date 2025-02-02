using RacingAidData.Core.Models;

namespace RacingAidWpf.Tracks.PositionCalculators;

public abstract class TrackMapPositionCalculator
{
    protected const float OneThousand = 1000f;
    
    public abstract TrackMapPosition CalculatePosition(TrackMapPosition previousPosition, DriverDataModel driverDataModel, float timeDeltaMs);
}