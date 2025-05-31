using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

public class ReplayController : IReplayControl
{
    private readonly IRecordData dataRecorder;
    private readonly IReplayData dataReplayer;

    public event Action<RaceDataModel>? ReplayDataReceived;
    public bool IsRecording => dataRecorder.IsRecording;
    public bool IsReplaying => dataReplayer.IsReplaying;

    public ReplayController(IRecordData? dataRecorder = null, IReplayData? dataReplayer = null)
    {
        this.dataRecorder = dataRecorder ?? new DataRecorder();
        this.dataReplayer = dataReplayer ?? new DataReplayer();

        this.dataReplayer.ReplayDataReceived += model => { ReplayDataReceived?.Invoke(model); };
    }

    public string StartRecording(string fileName = "")
    {
        if (IsRecording || IsReplaying)
            return string.Empty;
        
        return dataRecorder.Start(fileName);
    }

    public void RecordData(RaceDataModel data)
    {
        Task.Run(async () => await dataRecorder.AddRecordAsync(data));
    }

    public void StopRecording()
    {
        if (!IsRecording || IsReplaying)
            return;
        
        Task.Run(async () => await dataRecorder.StopAsync());
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