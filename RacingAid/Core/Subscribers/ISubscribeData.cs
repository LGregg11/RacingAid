using RacingAid.Core.Events;

namespace RacingAid.Core.Subscribers;

public interface ISubscribeData
{
    event EventHandler<DataReceivedEventArgs>? DataReceived;
    
    bool IsSubscribed { get; }
    
    void Start();
    void Stop();
}