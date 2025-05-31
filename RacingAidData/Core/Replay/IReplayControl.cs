using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

public interface IReplayControl
{
    public event Action<RaceDataModel> ReplayDataReceived;
    
    public bool IsRecording { get; }
    public bool IsReplaying { get; }
    
    public void StartRecording(string filePath);
    public void RecordData(RaceDataModel data);
    public Task StopRecordingAsync();

    public IList<string> GetReplays();
    public bool SelectReplay(string filePath);
    public void StartReplay();
    public void StopReplay();
}