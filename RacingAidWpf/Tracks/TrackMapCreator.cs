using RacingAidData.Core.Models;
using RacingAidWpf.Tracks.PositionCalculators;

namespace RacingAidWpf.Tracks;

/// <summary>
/// A class responsible for creating <see cref="TrackMap"/>s.
/// NOTE: This track map currently only creates track maps via the driver's speed and forward direction data - No other
/// modes of track creation currently exist.
/// </summary>
public class TrackMapCreator
{
    private TrackMapPositionCalculator positionCalculator;
    
    private TrackMap trackMapBeingCreated;
    private float previousLapsDriven;
    private bool isCurrentlyTrackingMap;

    public event Action<TrackMap> TrackCreated;
    
    public bool IsStarted { get; private set; }
    
    private TrackMapPositionCalculatorType positionCalculatorType = TrackMapPositionCalculatorType.SpeedAndDirection;
    public TrackMapPositionCalculatorType PositionCalculatorType
    {
        get => positionCalculatorType;
        set
        {
            if (positionCalculatorType == value)
                return;

            positionCalculatorType = value;
            OnTrackMapCalculatorTypeUpdated();
        }
    }

    public void Start(DriverDataModel driverDataModel, TrackDataModel trackDataModel)
    {
        trackMapBeingCreated = new TrackMap(trackDataModel.TrackName, CreateNewTrackMapPositions());
        
        previousLapsDriven = driverDataModel.LapsDriven;
        IsStarted = true;
    }

    public void Update(DriverDataModel driverDataModel)
    {
        if (!IsStarted)
            return;

        // When starting, we wait till the lap number changes to being tracking the position
        var currentLapsDriven = driverDataModel.LapsDriven;
        var lapNumberChanged = (int)currentLapsDriven > (int)previousLapsDriven;

        if (lapNumberChanged)
        {
            // Lap number has updated - toggle map tracking
            isCurrentlyTrackingMap = !isCurrentlyTrackingMap;

            if (!isCurrentlyTrackingMap)
            {
                End();
                return;
            }
        }

        if (isCurrentlyTrackingMap)
            UpdateTrack(driverDataModel);
        
        previousLapsDriven = currentLapsDriven;
    }

    public void Stop()
    {
        IsStarted = false;
        previousLapsDriven = 0;
        trackMapBeingCreated = null;
    }

    private void UpdateTrack(DriverDataModel driverDataModel)
    {
        var previousPosition = trackMapBeingCreated.Positions.Last();
        var newPosition = positionCalculator.CalculatePosition(previousPosition, driverDataModel);
        trackMapBeingCreated.Positions.Add(newPosition);
    }

    private void End()
    {
        IsStarted = false;
        trackMapBeingCreated.Positions = NormalizeAndCenterPositions(trackMapBeingCreated.Positions);
        TrackCreated?.Invoke(trackMapBeingCreated);
        
        trackMapBeingCreated = null;
    }
    
    private void OnTrackMapCalculatorTypeUpdated()
    {
        positionCalculator = positionCalculatorType switch
        {
            TrackMapPositionCalculatorType.SpeedAndDirection => new SpeedAndDirectionTrackMapCalculator(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private List<TrackMapPosition> NormalizeAndCenterPositions(List<TrackMapPosition> positions)
    {
        // TODO
        return positions;
    }

    private static List<TrackMapPosition> CreateNewTrackMapPositions()
    {
        // Start with an origin position
        return [ new TrackMapPosition(0f, 0f) ];
    }
}