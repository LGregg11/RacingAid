using System.Text.Json;
using RacingAidData.Core.Models;

namespace RacingAidData.Core.Replay;

/// <summary>
/// Record and store racing data in a newline-delimited json (.json1) file format
/// </summary>
public class DataRecorder : IRecordData
{
    private class FileExistsException(string message) : Exception(message);
    
    private const string DefaultFileExtension = ".json1";
    private const string DefaultFileNamePrefix = "RacingAidData";
    private const string DateTimeFormat = "yy-MM-dd_HH-mm-ss";
    
    private SemaphoreSlim? fileWriteSemaphore;
    private StreamWriter? streamWriter;
    
    private static string DefaultRecordDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "RacingAid", 
        "Recordings");
    
    public bool IsRecording { get; private set; }

    public string RecordDirectory => DefaultRecordDirectory;
    public string RecordExtension => DefaultFileExtension;
    
    public string Start(string fileName)
    {
        if (IsRecording)
            return string.Empty;

        string filePath;
        
        try
        {
            filePath = GetFilePath(fileName);
            OpenRecordingFile(filePath);
        }
        catch (Exception e)
        {
            // Raise error but continue
            Console.WriteLine(e);
            return string.Empty;
        }
        
        // Ensures only one write operation at a time
        fileWriteSemaphore = new SemaphoreSlim(1, 1);
        
        IsRecording = true;
        return filePath;
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
        var directory = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directory))
            return;
        
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        streamWriter = new StreamWriter(filePath);
    }

    private string GetFilePath(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            fileName = $"{DefaultFileNamePrefix}_{DateTime.Now.ToString(DateTimeFormat)}";

        fileName = Path.ChangeExtension(fileName, RecordExtension);
        var filePath = Path.Combine(RecordDirectory, fileName);

        if (File.Exists(filePath))
            throw new FileExistsException("File already exists");
        
        return filePath;
    }
}
