using Newtonsoft.Json;
using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

/// <summary>
/// Replay racing data stored in a newline-delimited json (.json1) file format
/// </summary>
public class DataReplayer : IReplayData
{
    private string replayFilePath = string.Empty;
    private Thread? replayThread;
    private CancellationTokenSource? cancellationTokenSource;
    private CancellationToken cancellationToken;

    private readonly Dictionary<ReplaySpeed, float> replaySpeedMap = new()
    {
        { ReplaySpeed.X1, 1f },
        { ReplaySpeed.X2, 1f / 2f },
        { ReplaySpeed.X4, 1f / 4f },
        { ReplaySpeed.X8, 1f / 8f },
        { ReplaySpeed.X16, 1f / 16f }
    };

    public event Action<RaceDataModel>? ReplayDataReceived;
    
    public ReplaySpeed ReplaySpeed { private get; set; }
    public bool IsSetup => !string.IsNullOrEmpty(replayFilePath);
    public bool IsReplaying => replayThread is { IsAlive: true };

    public void SetupReplay(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return;

        replayFilePath = filePath;
    }

    public void Start()
    {
        if (!IsSetup || IsReplaying)
            return;

        replayThread = new Thread(ReplayLoop);
        replayThread.Start();
    }

    public void Stop()
    {
        if (!IsReplaying)
            return;
        
        cancellationTokenSource?.Cancel();
        
        replayThread?.Join();
        cancellationTokenSource = null;
        replayThread = null;
    }

    private void ReplayLoop()
    {
        if (cancellationTokenSource == null)
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
        }
        
        using var replayDataEnumerator = ReadObjectsFromFile(replayFilePath).GetEnumerator();
        
        // Handle the first object outside the loop to get the start time
        // We use the timestamp so that we know when to send the next bit of data
        if (!replayDataEnumerator.MoveNext())
            return;
        
        var firstData = replayDataEnumerator.Current;
        var previousPacketTimestamp = firstData.Timestamp;

        ReplayDataReceived?.Invoke(firstData);
        var timeOfLastDataUpdate = DateTime.Now;

        while (!cancellationToken.IsCancellationRequested && replayDataEnumerator.MoveNext())
        {
            var currentData = replayDataEnumerator.Current;
            var currentDataTimestamp = currentData.Timestamp;

            var timestampDelta = currentDataTimestamp - previousPacketTimestamp;
            var timeSinceLastDataUpdate = DateTime.Now - timeOfLastDataUpdate;

            var timestampPlaybackDelta = timestampDelta * replaySpeedMap[ReplaySpeed];

            // If delay was not successfully completed, check if cancellation was requested
            var delaySuccess = DelayTillNextPacketSend(timestampPlaybackDelta, timeSinceLastDataUpdate);
            if (!delaySuccess && cancellationToken.IsCancellationRequested)
                break;
            
            ReplayDataReceived?.Invoke(currentData);
            timeOfLastDataUpdate = DateTime.Now;
            previousPacketTimestamp = currentDataTimestamp;
        }
    }

    private static IEnumerable<RaceDataModel> ReadObjectsFromFile(string filePath)
    {
        using var sr = new StreamReader(filePath);

        while (sr.ReadLine() is { } line)
        {
            // Ignore empty lines
            if (string.IsNullOrWhiteSpace(line))
                continue; 
            
            if (JsonConvert.DeserializeObject<RaceDataModel>(line, ReplaySettings.DefaultJsonSerializerSettings) is { } data)
                yield return data;
        }
    }

    /// <summary>
    /// Let's use the bool as a way to determine if the delay was successfully completed
    /// </summary>
    private bool DelayTillNextPacketSend(TimeSpan timestampDelta, TimeSpan timeSinceLastDataUpdate)
    {
        var dataDelay = timestampDelta.Subtract(timeSinceLastDataUpdate);

        if (dataDelay < TimeSpan.Zero)
            return false;
        
        Thread.Sleep(dataDelay);
        return true;
    }
}