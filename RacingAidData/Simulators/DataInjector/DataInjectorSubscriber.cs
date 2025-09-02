using RacingAidData.Core.Subscribers;
using RacingAidGrpc;

namespace RacingAidData.Simulators.DataInjector;

#if DEBUG
public class DataInjectorSubscriber : ISubscribeData
{
    private readonly TelemetryClient telemetryClient;
    
    public event Action? DataReceived;
    public event Action<bool>? ConnectionUpdated;
    public object? LatestData { get; private set; }
    public bool IsConnected => telemetryClient.IsConnected;
    public bool IsSubscribed => telemetryClient.IsStarted;

    public DataInjectorSubscriber()
    {
        telemetryClient = new TelemetryClient();
        telemetryClient.SessionStatusUpdated += OnStatusUpdated;
        telemetryClient.RelativeUpdated += OnRelativeUpdated;
    }

    private void OnRelativeUpdated(object? sender, RelativeResponse relative)
    {
        LatestData = relative;
        DataReceived?.Invoke();
    }

    private void OnStatusUpdated(object? sender, SessionStatusResponse status)
    {
        ConnectionUpdated?.Invoke(status.SessionActive);
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
#endif
