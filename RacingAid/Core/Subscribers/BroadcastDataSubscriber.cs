using System.Net.Sockets;
using RacingAid.Core.Client;
using RacingAid.Core.Events;

namespace RacingAid.Core.Subscribers;

public class BroadcastDataSubscriber : ISubscribeData
{
    private readonly IDataClient dataClient;
    private Thread? listenerThread;
    private bool keepThreadRunning;

    public BroadcastDataSubscriber(IDataClient dataClient)
    {
        this.dataClient = dataClient;
    }

    public event EventHandler<DataReceivedEventArgs>? DataReceived;

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
                    DataReceived?.Invoke(this, new DataReceivedEventArgs(receiveBytes));
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