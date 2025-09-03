using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace RacingAidGrpc;

public class TelemetryService : Telemetry.TelemetryBase
{
    private static readonly SubscriberStream<SessionStatusResponse> SessionStatusStream =
        new();
    
    private static readonly SubscriberStream<RelativeResponse> RelativeStream =
        new();

    public static async Task BroadcastSessionStatus(SessionStatusResponse sessionStatusResponse)
    {
        await SessionStatusStream.TryWriteAllAsync(sessionStatusResponse);
    }

    public static async Task BroadcastRelative(RelativeResponse relativeResponse)
    {
        await RelativeStream.TryWriteAllAsync(relativeResponse);
    }

    public override async Task SubscribeToSessionStatus(Empty request, IServerStreamWriter<SessionStatusResponse> responseStream, ServerCallContext context)
    {
        await SessionStatusStream.AddAndSendLastMessage(responseStream, context);
    }

    public override async Task SubscribeToRelative(Empty request, IServerStreamWriter<RelativeResponse> responseStream, ServerCallContext context)
    {
        await RelativeStream.AddAndSendLastMessage(responseStream, context);
    }
}