namespace RacingAidData.Core.Replay;

public class ReplayController
{
    private readonly IRecordData dataRecorder;
    private readonly IReplayData dataReplayer;
    
    public bool IsRecording => dataRecorder.IsRecording;
    public bool IsReplaying => dataReplayer.IsReplaying;

    public ReplayController(IRecordData? dataRecorder = null, IReplayData? dataReplayer = null)
    {
        this.dataRecorder = dataRecorder ?? new DataRecorder();
        this.dataReplayer = dataReplayer ?? new DataReplayer();
    }

    public void StartRecording(string filePath = "")
    {
        if (IsRecording || IsReplaying)
            return;
        
        var directory = Path.GetDirectoryName(filePath) ?? string.Empty;
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        
        dataRecorder.Start(directory, fileName);
    }

    public async Task StopRecordingAsync()
    {
        if (!IsRecording || IsReplaying)
            return;
        
        await dataRecorder.StopAsync();
    }

    public IList<string> GetReplays()
    {
        return new List<string>();
    }

    public void SelectReplay(string filePath)
    {
        dataReplayer.SetupReplay(filePath);
    }

    public void StartReplay()
    {
        if (IsRecording || IsReplaying)
            return;
        
        dataReplayer.Start();
    }

    public void StopReplay()
    {
        if (IsRecording || !IsReplaying)
            return;
        
        dataReplayer.Stop();
    }
}