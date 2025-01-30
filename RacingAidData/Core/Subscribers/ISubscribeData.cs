namespace RacingAidData.Core.Subscribers;

public interface ISubscribeData
{
    event Action? DataReceived;
    event Action<bool>? ConnectionUpdated;

    object? LatestData { get; }
    
    bool IsConnected { get; }
    bool IsSubscribed { get; }
    
    void Start();
    void Stop();
}