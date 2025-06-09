using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace RacingAidData.Core.Replay;

public class NewLineDelimitedJsonFileWriter<T> : IDisposable where T : class
{
    private const int QueueSizeWarningValue = 5;
    
    private readonly BlockingCollection<T> dataQueue = new();
    private readonly string filePath;
    private readonly Thread writerThread;
    private bool isActive;

    public NewLineDelimitedJsonFileWriter(string filePath)
    {
        this.filePath = filePath;
        writerThread = new Thread(WriteLoop)
        {
            IsBackground = true, // Allows the application to exit even if this thread is running
            Name = "NewLineDelimitedJsonFileWriter"
        };
        writerThread.Start();
        isActive = true;
    }

    public void EnqueueData(T data)
    {
        if (isActive)
            dataQueue.Add(data);
    }

    public void Dispose()
    {
        if (!isActive)
            return;

        isActive = false;
        dataQueue.CompleteAdding();
        writerThread.Join();
        dataQueue.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private void WriteLoop()
    {
        using var writer = new StreamWriter(filePath);
        
        // Blocks until an item is available or CompleteAdding is called
        foreach (var dataItem in dataQueue.GetConsumingEnumerable())
        {
            var queueCount = dataQueue.Count;
            if (queueCount >= QueueSizeWarningValue)
                Console.WriteLine($"Queue is getting large -> items: {queueCount}");

            var json = JsonConvert.SerializeObject(dataItem, ReplaySettings.DefaultJsonSerializerSettings);
            writer.WriteLine(json);
            writer.Flush();
        }
    }
}