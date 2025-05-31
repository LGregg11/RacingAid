using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

public interface IRecordData
{
    public bool IsRecording { get; }
    
    public string RecordDirectory { get; }
    public string RecordExtension { get; }
    
    public string Start(string fileName);
    public Task StopAsync();
    
    public Task AddRecordAsync(RaceDataModel raceDataRecord);
}