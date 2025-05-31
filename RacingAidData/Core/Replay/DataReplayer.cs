using System.Text.Json;
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

    public event Action<RaceDataModel>? ReplayDataReceived;
    
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

            // If delay was not successfully completed, check if cancellation was requested
            var delaySuccess = DelayTillNextPacketSend(timestampDelta, timeSinceLastDataUpdate);
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
            
            if (JsonSerializer.Deserialize<RaceDataModel>(line) is { } data)
                yield return data;
        }
    }

    /// <summary>
    /// Let's use the bool as a way to determine if the delay was successfully completed
    /// </summary>
    private bool DelayTillNextPacketSend(TimeSpan timestampDelta, TimeSpan timeSinceLastDataUpdate)
    {
        var dataDelay = timestampDelta.Subtract(timeSinceLastDataUpdate);

        try
        {
            Task.Delay(dataDelay, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return false;
        }

        return true;
    }
}