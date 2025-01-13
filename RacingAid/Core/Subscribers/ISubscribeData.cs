namespace RacingAid.Core.Subscribers;

public interface ISubscribeData<out T>
{
    event Action DataReceived;

    T LatestData { get; }
    
    bool IsSubscribed { get; }
    
    void Start();
    void Stop();
}