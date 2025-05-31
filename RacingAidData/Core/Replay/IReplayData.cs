using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

public interface IReplayData
{
    public event Action<RaceDataModel> ReplayDataReceived;
    public bool IsSetup { get; }
    public bool IsReplaying { get; }

    public void SetupReplay(string filePath);
    
    public void Start();
    public void Stop();
}