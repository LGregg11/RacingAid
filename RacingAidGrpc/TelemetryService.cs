using System.Threading.Channels;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace RacingAidGrpc;

public class TelemetryService : Telemetry.TelemetryBase
{
    private event EventHandler<bool> StatusUpdated;

    public void UpdateStatus(bool status)
    {
        StatusUpdated?.Invoke(this, status);
    }

    public override async Task SubscribeToSessionStatus(
        Empty _,
        IServerStreamWriter<SessionStatusResponse> responseStream,
        ServerCallContext context)
    {
        var channel = Channel.CreateUnbounded<bool>();
        
        StatusUpdated += async (_, active) =>
        {
            await channel.Writer.WriteAsync(active);
        };

        try
        {
            await foreach (var active in channel.Reader.ReadAllAsync(context.CancellationToken))
            {
                await responseStream.WriteAsync(new SessionStatusResponse { SessionActive = active });
            }
        }
        catch (OperationCanceledException)
        {
            // Do nothing I guess
        }
        finally
        {
            channel.Writer.TryComplete(); // Close the channel when done
        }
    }
}