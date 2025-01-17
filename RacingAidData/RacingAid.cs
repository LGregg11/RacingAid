using RacingAidData.Core.Deserializers;
using RacingAidData.Core.Models;
using RacingAidData.Core.Subscribers;
using RacingAidData.Simulators;
using RacingAidData.Simulators.iRacing;

namespace RacingAidData;

public class RacingAid
{
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
    
    #region Model Properties

    public DriversModel Drivers { get; private set; } = new();
    
    public TelemetryModel Telemetry { get; private set; } = new();
        
    #endregion

    public RacingAid()
    {
        SetupSimulator(Simulator.IRacing);
    }

    public void SetupSimulator(Simulator simulator)
    {
        switch (simulator)
        {
            case Simulator.F1:
                break;
            case Simulator.IRacing:
                DataDeserializer = new iRacingDataDeserializer();
                DataSubscriber = new iRacingDataSubscriber();
                break;
        }
    }

    public void Start()
    {
        if (DataSubscriber == null)
            return;
        
        DataSubscriber.DataReceived += OnDataReceived;
        DataSubscriber?.Start();
    }

    public void Stop()
    {
        if (DataSubscriber == null)
            return;
        
        DataSubscriber.Stop();
        DataSubscriber.DataReceived -= OnDataReceived;
    }

    private void OnDataReceived()
    {
        if (DataSubscriber?.LatestData == null || DataDeserializer == null)
            return;
        
        if (!DataDeserializer.TryDeserializeData(DataSubscriber.LatestData, out var models))
            return;

        foreach (RaceDataModel model in models)
            UpdateProperty(model);
    }

    private void UpdateProperty(RaceDataModel model)
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