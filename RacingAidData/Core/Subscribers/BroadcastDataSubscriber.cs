using System.Net.Sockets;
using RacingAidData.Core.Client;

namespace RacingAidData.Core.Subscribers;

public class BroadcastDataSubscriber(IDataClient dataClient) : ISubscribeData
{
    private Thread? listenerThread;
    private bool keepThreadRunning;

    public event Action? DataReceived;
    
    public event Action<bool>? ConnectionUpdated;
    
    public object? LatestData { get; private set; }

    /// <remarks>
    /// Just assume we are connected when we are subscribed - its hard to tell with UDP broadcasts
    /// </remarks>
    public bool IsConnected => IsSubscribed;

    public bool IsSubscribed => listenerThread is { IsAlive: true };

    public void Start()
    {
        if (IsSubscribed)
            return;
        

        dataClient.Start();
        StartThread();
        
        ConnectionUpdated?.Invoke(IsConnected);
    }

    public void Stop()
    {
        if (!IsSubscribed)
            return;

        dataClient.Stop();
        StopThread();
        
        ConnectionUpdated?.Invoke(IsConnected);
    }

    private void DataSubscriber()
    {
        while (keepThreadRunning)
        {
            try
            {
                byte[]? receiveBytes = dataClient?.Receive();
                if (receiveBytes is { Length: > 0 })
                {
                    LatestData = receiveBytes;
                    DataReceived?.Invoke();
                }
            }
            catch (SocketException)
            {
                // Client closed - stop the thread
                keepThreadRunning = false;
            }
        }
    }

    private void StartThread()
    {
        keepThreadRunning = true;

        listenerThread = new Thread(DataSubscriber) { Name = $"{nameof(BroadcastDataSubscriber)} Thread" };
        listenerThread.Start();
    }

    private void StopThread()
    {
        keepThreadRunning = false;
        listenerThread?.Join();
    }
}