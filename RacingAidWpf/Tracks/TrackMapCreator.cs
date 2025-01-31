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
    private DateTime lastUpdateTime;
    private float previousLapsDriven;
    private bool isCurrentlyTrackingMap;
    private int incidentsAtTrackingStart = 0;

    public event Action<TrackMap> TrackCreated;
    
    public bool IsStarted { get; private set; }
    
    private TrackMapPositionCalculatorType positionCalculatorType;
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

    public TrackMapCreator()
    {
        PositionCalculatorType = TrackMapPositionCalculatorType.VelocityAndDirection;
        OnTrackMapCalculatorTypeUpdated();
    }

    public void Start(DriverDataModel driverDataModel, TrackDataModel trackDataModel)
    {
        if (IsStarted)
            Stop();
        
        Console.WriteLine($"Starting track map creation for: {trackDataModel.TrackName}");
        
        trackMapBeingCreated = new TrackMap(trackDataModel.TrackName, CreateNewTrackMapPositions());
        previousLapsDriven = driverDataModel.LapsDriven;
        lastUpdateTime = driverDataModel.Timestamp;
        isCurrentlyTrackingMap = false;
        IsStarted = true;
    }

    public void Update(DriverDataModel driverDataModel)
    {
        if (!IsStarted)
            return;

        // When starting, we wait till the lap number changes to being tracking the position
        var currentLapsDriven = driverDataModel.LapsDriven;
        
        var lapCompleted = (int)currentLapsDriven > (int)previousLapsDriven;
        
        if (lapCompleted)
            OnLapCompleted(driverDataModel);

        if (isCurrentlyTrackingMap)
            UpdateTrack(driverDataModel);
        
        previousLapsDriven = currentLapsDriven;
        lastUpdateTime = driverDataModel.Timestamp;
    }

    public void Stop()
    {
        if (!IsStarted)
            return;
        
        Console.WriteLine($"Stopping track map creation for: {trackMapBeingCreated.Name}");
        
        IsStarted = false;
        previousLapsDriven = 0;
        trackMapBeingCreated = null;
    }

    private void UpdateTrack(DriverDataModel driverDataModel)
    {
        var previousPosition = trackMapBeingCreated.Positions.Last();
        var timeDeltaMs = (float)(driverDataModel.Timestamp - lastUpdateTime).TotalMilliseconds;
        var newPosition = positionCalculator.CalculatePosition(previousPosition, driverDataModel, timeDeltaMs);
        trackMapBeingCreated.Positions.Add(newPosition);
    }

    private void OnLapCompleted(DriverDataModel driverDataModel)
    {
        if (!isCurrentlyTrackingMap)
        {
            if (driverDataModel.InPits)
                Console.WriteLine("Tracking has not started, but driver is currently in pits - ignore this lap");
            else
            {
                isCurrentlyTrackingMap = true;
                incidentsAtTrackingStart = driverDataModel.Incidents;
            }
            
            return;
        }

        var currentIncidents = driverDataModel.Incidents;
        var incidentsThisLap = currentIncidents - incidentsAtTrackingStart;
        
        // Tracking was already underway - was the lap valid (i.e. didn't end in pits & 0 incidents gained)
        var inPits = driverDataModel.InPits;
        var hadIncidentsOnLap = incidentsThisLap > 0;

        if (inPits || hadIncidentsOnLap)
        {
            if (inPits)
            {
                Console.WriteLine("Invalid lap - Driver ended the lap in pits");
                isCurrentlyTrackingMap = false;
            }

            if (hadIncidentsOnLap)
            {
                Console.WriteLine($"Invalid lap - Driver had {incidentsThisLap} incidents this lap");
                incidentsAtTrackingStart = currentIncidents;
            }
            
            Console.WriteLine("Resetting data");
            trackMapBeingCreated.Positions = CreateNewTrackMapPositions();
            return;
        }

        isCurrentlyTrackingMap = false;
        End();
    }

    private void End()
    {
        Console.WriteLine($"Ending track map creation for: {trackMapBeingCreated.Name}");
        
        IsStarted = false;
        trackMapBeingCreated.Positions = CenterPositions(trackMapBeingCreated.Positions);
        TrackCreated?.Invoke(trackMapBeingCreated);
        
        trackMapBeingCreated = null;
    }
    
    private void OnTrackMapCalculatorTypeUpdated()
    {
        positionCalculator = positionCalculatorType switch
        {
            TrackMapPositionCalculatorType.VelocityAndDirection => new VelocityAndDirectionTrackMapCalculator(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private static List<TrackMapPosition> CenterPositions(List<TrackMapPosition> positions)
    {
        var nPositions = positions.Count;
        var xAvg = 0f;
        var yAvg = 0f;
        var zAvg = 0f;

        foreach (var position in positions)
        {
            xAvg += position.X;
            yAvg += position.Y;
            zAvg += position.Z;
        }
        
        xAvg /= nPositions;
        yAvg /= nPositions;
        zAvg /= nPositions;

        return positions.Select(position =>
                new TrackMapPosition(position.LapDistance, position.X - xAvg, position.Y - yAvg, position.Z - zAvg))
            .ToList();
    }

    private static List<TrackMapPosition> CreateNewTrackMapPositions()
    {
        // Start with an origin position
        return [ new TrackMapPosition(0f ,0f, 0f, 0f) ];
    }
}