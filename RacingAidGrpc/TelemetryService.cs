using System.Collections.Concurrent;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace RacingAidGrpc;

public class TelemetryService(ILogger<TelemetryService> logger) : Telemetry.TelemetryBase
{
    private static readonly ConcurrentDictionary<string, IServerStreamWriter<SessionStatusResponse>> ActiveStreams =
        new();

    private static bool currentSessionStatus;

    public override async Task SubscribeToSessionStatus(Empty request, IServerStreamWriter<SessionStatusResponse> responseStream, ServerCallContext context)
    {
        var connectionId = Guid.NewGuid().ToString();
        logger.LogInformation("Client connected. Connection ID: {ConnectionId}", connectionId);

        ActiveStreams.TryAdd(connectionId, responseStream);

        try
        {
            // Immediately send the current session status to the newly connected client.
            await responseStream.WriteAsync(new SessionStatusResponse { SessionActive = currentSessionStatus });

            // Keep the method alive until the client disconnects or the server shuts down.
            await Task.Delay(Timeout.Infinite, context.CancellationToken);
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Client with ID {ConnectionId} disconnected from session status stream.", connectionId);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            logger.LogInformation("Client with ID {ConnectionId} cancelled the gRPC call: {Message}", connectionId, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in session status stream for client ID {ConnectionId}", connectionId);
        }
        finally
        {
            ActiveStreams.TryRemove(connectionId, out _);
            logger.LogInformation("Client with ID {ConnectionId} stream cleaned up. Remaining active streams: {Count}", connectionId, ActiveStreams.Count);
        }
    }

    public static async Task BroadcastSessionStatus(bool newStatus)
    {
        // Update the static current status so new clients get the latest state.
        currentSessionStatus = newStatus;

        var response = new SessionStatusResponse { SessionActive = newStatus };
        List<string> disconnectedClientIds = [];

        // Iterate through all active streams and send the update.
        foreach (var (connectionId, writer) in ActiveStreams)
        {
            try
            {
                await writer.WriteAsync(response);
            }
            catch (Exception ex)
            {
                // If writing fails, it likely means the client has disconnected unexpectedly.
                // Mark this client for removal.
                Console.WriteLine($"Failed to send update to client {connectionId}: {ex.Message}");
                disconnectedClientIds.Add(connectionId);
            }
        }

        // Clean up any streams that failed to send messages.
        foreach (var id in disconnectedClientIds)
        {
            ActiveStreams.TryRemove(id, out _);
            Console.WriteLine($"Removed disconnected client: {id}. Remaining active streams: {ActiveStreams.Count}");
        }
    }
}