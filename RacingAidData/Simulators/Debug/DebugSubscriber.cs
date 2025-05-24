using RacingAidData.Core.Subscribers;
using RacingAidGrpc;

namespace RacingAidData.Simulators.Debug;

public class DebugSubscriber : ISubscribeData
{
    private readonly TelemetryClient telemetryClient;
    
    public event Action? DataReceived;
    public event Action<bool>? ConnectionUpdated;
    public object? LatestData { get; private set; }
    public bool IsConnected => telemetryClient.IsConnected;
    public bool IsSubscribed => telemetryClient.IsStarted;

    public DebugSubscriber()
    {
        telemetryClient = new TelemetryClient();
        telemetryClient.SessionStatusUpdated += OnStatusUpdated;
    }

    private void OnStatusUpdated(object? sender, bool status)
    {
        ConnectionUpdated?.Invoke(status);
    }

    public void Start()
    {
        telemetryClient.Start();
    }

    public void Stop()
    {
        telemetryClient.Stop();
    }
}