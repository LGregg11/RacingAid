using IRSDKSharper;
using RacingAid.Core.Subscribers;

namespace RacingAid.Simulators.iRacing;

public class iRacingDataSubscriber : ISubscribeData<IRacingSdkData>
{
    private readonly IRacingSdk iRacingSdk;
    
    public event Action? DataReceived;

    public IRacingSdkData? LatestData { get; private set; }

    public bool IsSubscribed => iRacingSdk.IsStarted;

    public iRacingDataSubscriber()
    {
        iRacingSdk = new IRacingSdk
        {
            UpdateInterval = 30
        };

        iRacingSdk.OnTelemetryData += OnTelemetryData;
    }

    public void Start()
    {
        iRacingSdk.Start();
    }

    public void Stop()
    {
        iRacingSdk.Stop();
    }

    private void OnTelemetryData()
    {
        LatestData = iRacingSdk.Data;
        DataReceived?.Invoke();
    }
}