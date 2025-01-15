namespace RacingAidData.Core.Subscribers;

public interface ISubscribeData
{
    event Action? DataReceived;

    object? LatestData { get; }
    
    bool IsSubscribed { get; }
    
    void Start();
    void Stop();
}