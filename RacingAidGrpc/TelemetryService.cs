using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace RacingAidGrpc;

public class TelemetryService : Telemetry.TelemetryBase
{
    private readonly List<IServerStreamWriter<SessionStatusResponse>> subscribers = [];
    private readonly object lockObject = new();

    public override async Task SubscribeToSessionStatus(
        Empty _,
        IServerStreamWriter<SessionStatusResponse> responseStream,
        ServerCallContext context)
    {
        lock (lockObject)
        {
            subscribers.Add(responseStream);
        }

        try
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000); // Keep connection alive
            }
        }
        finally
        {
            lock (lockObject)
            {
                subscribers.Remove(responseStream);
            }
        }
    }
    
    public async Task BroadcastSessionStatus(bool sessionActive)
    {
        lock (lockObject)
        {
            foreach (var subscriber in subscribers)
            {
                subscriber.WriteAsync(new SessionStatusResponse { SessionActive = sessionActive });
            }
        }
    }
}