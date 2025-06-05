using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

public interface IReplayControl
{
    public event Action<RaceDataModel> ReplayDataReceived;
    public event Action<bool> IsReplayingUpdated;
    
    public bool IsRecording { get; }
    public bool IsReplaying { get; }
    
    public ReplaySpeed ReplaySpeed { set; }
    
    public string StartRecording(string filePath);
    public void RecordData(RaceDataModel data);
    public void StopRecording();

    public IList<string> GetReplays();
    public bool SelectReplay(string filePath);
    public void StartReplay();
    public void StopReplay();
}