using System.Net.Sockets;
using RacingAid.Core.Client;

namespace RacingAid.Core.Subscribers;

public class BroadcastDataSubscriber : ISubscribeData<byte[]>
{
    private readonly IDataClient dataClient;
    private Thread? listenerThread;
    private bool keepThreadRunning;

    public BroadcastDataSubscriber(IDataClient dataClient)
    {
        this.dataClient = dataClient;
    }

    public event Action?  DataReceived;
    
    public byte[]?  LatestData { get; private set; }

    public bool IsSubscribed => listenerThread is { IsAlive: true };

    public void Start()
    {
        if (IsSubscribed)
            return;

        dataClient.Start();
        StartThread();
    }

    public void Stop()
    {
        if (!IsSubscribed)
            return;

        dataClient.Stop();
        StopThread();
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