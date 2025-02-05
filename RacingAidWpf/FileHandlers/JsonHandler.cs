using System.IO;
using Newtonsoft.Json;
using RacingAidWpf.Logging;

namespace RacingAidWpf.FileHandlers;

public class JsonHandler<T> : IHandleData<T> where T : class
{
    private readonly ILogger logger;
    
    public JsonHandler(ILogger logger = null)
    {
        this.logger = logger;
    }
    
    public bool TryDeserializeFromFile(string filePath, out T data)
    {
        data = default;

        if (!File.Exists(filePath))
        {
            logger?.LogWarning($"File '{filePath}' does not exist");
            return false;
        }
        
        var jsonStr = File.ReadAllText(filePath);

        if (JsonConvert.DeserializeObject<T>(jsonStr) is { } deserializedData)
            data = deserializedData;

        return data != null;
    }

    public bool TrySerializeToFile(string filePath, T data)
    {
        var jsonStr = JsonConvert.SerializeObject(data, Formatting.Indented);
        if (string.IsNullOrEmpty(jsonStr))
        {
            logger?.LogWarning($"Serialized data for {typeof(T)} is empty");
            return false;
        }

        var directory = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directory))
        {
            logger?.LogError($"Failed to find directory for {filePath}");
            return false;
        }

        if (!Directory.Exists(directory))
        {
            logger?.LogDebug($"Creating directory '{directory}'");
            Directory.CreateDirectory(directory);
        }
        
        File.WriteAllTextAsync(filePath, jsonStr);
        return true;
    }
}
