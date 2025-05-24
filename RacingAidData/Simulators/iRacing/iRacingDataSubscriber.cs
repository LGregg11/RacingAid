using IRSDKSharper;
using RacingAidData.Core.Subscribers;

namespace RacingAidData.Simulators.iRacing;

public class iRacingDataSubscriber : ISubscribeData
{
    private readonly IRacingSdk iRacingSdk;
    
    public event Action? DataReceived;
    
    public event Action<bool>? ConnectionUpdated;

    public object? LatestData { get; private set; }
    
    public bool IsConnected => iRacingSdk.IsConnected;

    public bool IsSubscribed => iRacingSdk.IsStarted;

    public iRacingDataSubscriber()
    {
        iRacingSdk = new IRacingSdk();

        iRacingSdk.OnTelemetryData += OnTelemetryData;
        iRacingSdk.OnConnected += OnConnected;
        iRacingSdk.OnDisconnected += OnDisconnected;
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

    private void OnConnected()
    {
        ConnectionUpdated?.Invoke(IsConnected);
    }

    private void OnDisconnected()
    {
        ConnectionUpdated?.Invoke(IsConnected);
    }
}