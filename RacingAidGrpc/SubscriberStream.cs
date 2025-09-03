#nullable enable
using System.Collections.Concurrent;
using Grpc.Core;

namespace RacingAidGrpc;

public class SubscriberStream<T> where T : class
{
    private readonly ConcurrentDictionary<string, IServerStreamWriter<T>> activeStreams =
        new();

    private T? lastMessage;
    
    public async Task AddAndSendLastMessage(IServerStreamWriter<T> stream, ServerCallContext context)
    {
        var id = Guid.NewGuid().ToString();
        activeStreams.TryAdd(id, stream);
        
        try
        {
            // Immediately send the current session status to the newly connected client.
            if (lastMessage != null)
                await stream.WriteAsync(lastMessage);

            // Keep the method alive until the client disconnects or the server shuts down.
            await Task.Delay(Timeout.Infinite, context.CancellationToken);
        }
        catch (Exception)
        {
            activeStreams.TryRemove(id, out _);
        }
    }

    public async Task TryWriteAllAsync(T message)
    {
        foreach (var id in activeStreams.Keys)
            await TryWriteAsync(id, message);
        
        lastMessage = message;
    }
    
    public async Task TryWriteAsync(string id, T message)
    {
        if (!activeStreams.TryGetValue(id, out var stream))
            return;
        
        List<string> disconnectedClientIds = [];
        
        try
        {
            await stream.WriteAsync(message);
        }
        catch (Exception)
        {
            disconnectedClientIds.Add(id);
        }
        
        foreach (var disconnectedClientId in disconnectedClientIds)
            activeStreams.TryRemove(disconnectedClientId, out _);
    }
}