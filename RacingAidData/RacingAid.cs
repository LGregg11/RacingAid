using RacingAidData.Core.Deserializers;
using RacingAidData.Core.Models;
using RacingAidData.Core.Subscribers;
using RacingAidData.Simulators;
using RacingAidData.Simulators.iRacing;

namespace RacingAidData;

public class RacingAid
{
    private bool modelsHaveUpdated;
    private bool isRunning;
    
    private IDeserializeData? DataDeserializer { get; set; }

    private ISubscribeData? dataSubscriber;
    private ISubscribeData? DataSubscriber
    {
        get => dataSubscriber;
        set
        {
            if (dataSubscriber == value)
                return;

            var subscriberRunning = dataSubscriber is { IsSubscribed: true };
            if (subscriberRunning)
                Stop();
            
            dataSubscriber = value;

            if (subscriberRunning)
                Start();
        }
    }

    public event Action<bool> ConnectionUpdated;

    public event Action ModelsUpdated;
    
    #region Model Properties

    private TimesheetModel timesheet = new();

    public TimesheetModel Timesheet
    {
        get => timesheet;
        private set
        {
            if (timesheet == value)
                return;
            
            timesheet = value;
            modelsHaveUpdated = true;
        }
    }

    private RelativeModel relative = new();

    public RelativeModel Relative
    {
        get => relative;
        private set
        {
            if (relative == value)
                return;
            
            relative = value;
            modelsHaveUpdated = true;
        }
    }
    
    private TelemetryModel telemetry = new();

    public TelemetryModel Telemetry
    {
        get => telemetry;
        private set
        {
            if (telemetry == value)
                return;
            
            telemetry = value;
            modelsHaveUpdated = true;
        }
    }
    
    private DriverDataModel driverData = new();

    public DriverDataModel DriverData
    {
        get => driverData;
        private set
        {
            if (driverData == value)
                return;
            
            driverData = value;
            modelsHaveUpdated = true;
        }
    }
    
    private TrackDataModel trackData = new();

    public TrackDataModel TrackData
    {
        get => trackData;
        private set
        {
            if (trackData == value)
                return;
            
            trackData = value;
            modelsHaveUpdated = true;
        }
    }
    
    #endregion

    public bool IsConnected => DataSubscriber is { IsConnected: true };

    public RacingAid()
    {
        SetupSimulator(Simulator.iRacing);
    }

    public void SetupSimulator(Simulator simulator)
    {
        switch (simulator)
        {
            case Simulator.F1:
                break;
            case Simulator.iRacing:
                DataDeserializer = new iRacingDataDeserializer();
                DataSubscriber = new iRacingDataSubscriber();
                break;
        }
    }

    public void Start()
    {
        if (isRunning || DataSubscriber == null)
            return;
        
        DataSubscriber.DataReceived += OnDataReceived;
        DataSubscriber?.Start();
        isRunning = true;
    }

    public void Stop()
    {
        if (!isRunning || DataSubscriber == null)
            return;
        
        DataSubscriber.Stop();
        DataSubscriber.DataReceived -= OnDataReceived;
        isRunning = false;
    }

    private void OnConnectionUpdated(bool connected)
    {
        ConnectionUpdated?.Invoke(connected);
    }

    private void OnDataReceived()
    {
        if (DataSubscriber?.LatestData == null || DataDeserializer == null)
            return;
        
        if (!DataDeserializer.TryDeserializeData(DataSubscriber.LatestData, out var models))
            return;

        foreach (var model in models)
            UpdateModel(model);

        if (modelsHaveUpdated)
            ModelsUpdated?.Invoke();
        
        modelsHaveUpdated = false;
    }

    private void UpdateModel(RaceDataModel model)
    {
        switch (model)
        {
            case TimesheetModel driversModel:
                Timesheet = driversModel;
                break;
            case TelemetryModel telemetryModel:
                Telemetry = telemetryModel;
                break;
            case RelativeModel relativeModel:
                Relative = relativeModel;
                break;
            case DriverDataModel driverDataModel:
                DriverData = driverDataModel;
                break;
            case TrackDataModel trackDataModel:
                TrackData = trackDataModel;
                break;
        }
    }
}