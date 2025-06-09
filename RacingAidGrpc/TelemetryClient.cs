using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace RacingAidGrpc;

public class TelemetryClient(GrpcChannel channel)
{
    private readonly Telemetry.TelemetryClient telemetryClient = new(channel);
    
    private CancellationTokenSource cancellationTokenSource;
    private Task sessionStatusSubscriptionTask;

    public bool IsStarted { get; private set; }
    
    public bool IsConnected { get; private set; }
    
    public event EventHandler<bool> SessionStatusUpdated;

    public TelemetryClient() : this(Utils.DefaultHost, Utils.DefaultPort)
    {
    }

    public TelemetryClient(string host, string port) : this(GrpcChannel.ForAddress($"{host}:{port}"))
    {
    }

    public void Start()
    {
        if (IsStarted)
            return;
        
        cancellationTokenSource = new CancellationTokenSource();
        sessionStatusSubscriptionTask = Task.Run(() => SubscribeToSessionStatus(telemetryClient, cancellationTokenSource.Token));
        IsStarted = true;
    }

    public void Stop()
    {
        if (!IsStarted)
            return;
        
        cancellationTokenSource.Cancel();
        sessionStatusSubscriptionTask.Wait();
        IsStarted = false;
    }

    private async Task SubscribeToSessionStatus(Telemetry.TelemetryClient client, CancellationToken cancellationToken)
    {
        try
        {
            using var call = client.SubscribeToSessionStatus(new Empty(), cancellationToken: cancellationToken);

            await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                IsConnected = response.SessionActive;
                SessionStatusUpdated?.Invoke(this, response.SessionActive);
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            Console.WriteLine("Stream cancelled.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}