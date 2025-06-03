using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

public interface IRecordData
{
    public bool IsRecording { get; }
    
    public string RecordDirectory { get; }
    public string RecordExtension { get; }
    
    public string Start(string fileName);
    public void Stop();
    public void AddRecord(RaceDataModel raceData);
}