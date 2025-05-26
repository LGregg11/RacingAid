using System.Text;
using System.Text.Json;
using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

public class DataRecorder : IRecordData
{
    private class FileExistsException(string message) : Exception(message);
    
    private const string DateTimeFormat = "YY-MM-dd_HH-mm-ss";
    private const string DefaultFileNamePrefix = "RacingAidData";
    
    private SemaphoreSlim? fileWriteSemaphore;
    private FileStream? fileStream;
    
    private static string DefaultRecordDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Recordings");
    
    public bool IsRecording { get; private set; }
    
    public void Start(string filePath)
    {
        if (IsRecording)
            return;
        
        try
        {
            filePath = GetFilePath(filePath);
        }
        catch (Exception e)
        {
            // Raise error but continue
            Console.WriteLine(e);
            return;
        }
        
        // Ensures only one write operation at a time
        fileWriteSemaphore = new SemaphoreSlim(1, 1);
        
        OpenRecordingFile(filePath);
        IsRecording = true;
    }

    public async Task StopAsync()
    {
        if (!IsRecording)
            return;
        
        await CloseRecordingFileAsync();
        IsRecording = false;
    }

    private async Task CloseRecordingFileAsync()
    {
        if (fileStream == null || fileWriteSemaphore == null)
            return;
        
        await fileStream.FlushAsync();
        await fileStream.DisposeAsync();
        fileStream = null;
        
        fileWriteSemaphore?.Dispose();
        fileWriteSemaphore = null;
    }

    public async Task AddRecordAsync(RaceDataModel raceDataRecord)
    {
        if (fileStream == null || fileWriteSemaphore == null)
            return;

        await fileWriteSemaphore.WaitAsync();

        var bytes = ModelToBytes(raceDataRecord);
        
        await fileStream.WriteAsync(bytes);
        await fileStream.FlushAsync();
        
        fileWriteSemaphore.Release();
    }

    private void OpenRecordingFile(string filePath)
    {
        fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
    }

    private static string GetFilePath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            filePath = Path.Combine(DefaultRecordDirectory, DefaultFileNamePrefix);
        
        filePath += $"_{DateTime.Now.ToString(DateTimeFormat)}";

        if (File.Exists(filePath))
            throw new FileExistsException("File already exists");
        
        return filePath;
    }

    private static byte[] ModelToBytes(RaceDataModel raceDataRecord)
    {
        var json = JsonSerializer.Serialize(raceDataRecord);
        return Encoding.UTF8.GetBytes(json + Environment.NewLine);
    }
}
