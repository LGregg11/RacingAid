using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

public interface IRecordData
{
    public bool IsRecording { get; }
    
    public void Start(string directory, string filename);
    public Task StopAsync();
    
    public Task AddRecordAsync(RaceDataModel raceDataRecord);
}