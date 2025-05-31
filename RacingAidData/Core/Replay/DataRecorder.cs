using System.Text.Json;
using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

/// <summary>
/// Record and store racing data in a newline-delimited json (.json1) file format
/// </summary>
public class DataRecorder : IRecordData
{
    private class FileExistsException(string message) : Exception(message);
    
    private const string FileExtension = ".json1";
    
    private const string DateTimeFormat = "YY-MM-dd_HH-mm-ss";
    private const string DefaultFileNamePrefix = "RacingAidData";
    
    private SemaphoreSlim? fileWriteSemaphore;
    private StreamWriter? streamWriter;
    
    private static string DefaultRecordDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Recordings");
    
    public bool IsRecording { get; private set; }
    
    public void Start(string directory, string fileName)
    {
        if (IsRecording)
            return;

        string filePath;
        
        try
        {
            filePath = GetFilePath(directory, fileName);
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
        if (streamWriter == null || fileWriteSemaphore == null)
            return;
        
        await streamWriter.FlushAsync();
        await streamWriter.DisposeAsync();
        streamWriter = null;
        
        fileWriteSemaphore?.Dispose();
        fileWriteSemaphore = null;
    }

    public async Task AddRecordAsync(RaceDataModel raceDataRecord)
    {
        if (streamWriter == null || fileWriteSemaphore == null)
            return;

        await fileWriteSemaphore.WaitAsync();

        var jsonString = JsonSerializer.Serialize(raceDataRecord);
        await streamWriter.WriteLineAsync(jsonString);
        await streamWriter.FlushAsync();
        
        fileWriteSemaphore.Release();
    }

    private void OpenRecordingFile(string filePath)
    {
        streamWriter = new StreamWriter(filePath);
    }

    private static string GetFilePath(string directory, string fileName)
    {
        var filePath = Path.Combine(directory, fileName);
        
        if (string.IsNullOrEmpty(filePath))
            filePath = Path.Combine(DefaultRecordDirectory, DefaultFileNamePrefix);
        
        filePath += $"_{DateTime.Now.ToString(DateTimeFormat)}{FileExtension}";

        if (File.Exists(filePath))
            throw new FileExistsException("File already exists");
        
        return filePath;
    }
}
