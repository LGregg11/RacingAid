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

    public void StartRecording(string fileName = "")
    {
        if (IsRecording || IsReplaying)
            return;
        
        dataRecorder.Start(fileName);
    }

    public async Task StopRecordingAsync()
    {
        if (!IsRecording || IsReplaying)
            return;
        
        await dataRecorder.StopAsync();
    }

    public IList<string> GetReplays()
    {
        return Directory.GetFiles(dataRecorder.RecordDirectory, $"*{dataRecorder.RecordExtension}");
    }

    public bool SelectReplay(string filePath)
    {
        if (!GetReplays().Contains(filePath))
            return false;
        
        dataReplayer.SetupReplay(filePath);
        return true;

    }

    public void StartReplay()
    {
        if (IsRecording || IsReplaying || !dataReplayer.IsSetup)
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