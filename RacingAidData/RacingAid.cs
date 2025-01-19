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

    public event Action ModelsUpdated;
    
    #region Model Properties

    private DriversModel drivers = new();

    public DriversModel Drivers
    {
        get => drivers;
        private set
        {
            if (drivers == value)
                return;
            
            drivers = value;
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
    
    #endregion

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

    private void OnDataReceived()
    {
        if (DataSubscriber?.LatestData == null || DataDeserializer == null)
            return;
        
        if (!DataDeserializer.TryDeserializeData(DataSubscriber.LatestData, out var models))
            return;

        foreach (RaceDataModel model in models)
            UpdateModel(model);

        if (modelsHaveUpdated)
            ModelsUpdated?.Invoke();
        
        modelsHaveUpdated = false;
    }

    private void UpdateModel(RaceDataModel model)
    {
        switch (model)
        {
            case DriversModel driversModel:
                Drivers = driversModel;
                break;
            case TelemetryModel telemetryModel:
                Telemetry = telemetryModel;
                break;
        }
    }
}