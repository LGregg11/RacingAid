using System.Text.Json;
using Newtonsoft.Json;
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
    
    private NewLineDelimitedJsonFileWriter<RaceDataModel>? dataFileWriter;
    
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
        
        IsRecording = true;
        return filePath;
    }

    public void Stop()
    {
        if (!IsRecording)
            return;
        
        CloseRecordingFile();
        IsRecording = false;
    }

    private void CloseRecordingFile()
    {
        dataFileWriter?.Dispose();
        dataFileWriter = null;
    }

    public void AddRecord(RaceDataModel raceData)
    {
        dataFileWriter?.EnqueueData(raceData);
    }

    private void OpenRecordingFile(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directory))
            return;
        
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        dataFileWriter = new NewLineDelimitedJsonFileWriter<RaceDataModel>(filePath);
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
