namespace RacingAidData.Core.Client;

public interface IDataClient : IDisposable
{
    public void Start();
    public void Stop();
    public byte[]? Receive();
}