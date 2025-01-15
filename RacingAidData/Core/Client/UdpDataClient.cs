using System.Net;
using System.Net.Sockets;

namespace RacingAidData.Core.Client;

public class UdpDataClient(int port, IPAddress ipAddress) : IDataClient
{
    private UdpClient? udpClient;

    public int Port { get; set; } = port;
    
    public IPAddress IpAddress { get; set; } = ipAddress;

    public void Start()
    {
        udpClient ??= new UdpClient(Port);
    }

    public void Stop()
    {
        Close();
        Dispose();
    }

    public void Dispose()
    {
        udpClient?.Dispose();
        udpClient = null;
        GC.SuppressFinalize(this);
    }

    public byte[]? Receive()
    {
        IPEndPoint remoteEndPoint = GetRemoteEndPoint();
        return udpClient?.Receive(ref remoteEndPoint);
    }

    private IPEndPoint GetRemoteEndPoint() => new(IpAddress, Port);

    private void Close()
    {
        udpClient?.Close();
    }
}