using System.Collections.Concurrent;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace RacingAidGrpc;

public class TelemetryService(ILogger<TelemetryService> logger) : Telemetry.TelemetryBase
{
    private static readonly SubscriberStream<SessionStatusResponse> SessionStatusStream =
        new();

    public override async Task SubscribeToSessionStatus(Empty request, IServerStreamWriter<SessionStatusResponse> responseStream, ServerCallContext context)
    {
        await SessionStatusStream.AddAndSendLastMessage(responseStream, context);
    }

    public static async Task BroadcastSessionStatus(bool newStatus)
    {
        var response = new SessionStatusResponse { SessionActive = newStatus };
        
        await SessionStatusStream.TryWriteAllAsync(response);
    }
}