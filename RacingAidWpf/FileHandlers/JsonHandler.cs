using System.IO;
using Newtonsoft.Json;

namespace RacingAidWpf.FileHandlers;

public class JsonHandler<T> : IHandleData<T> where T : class
{
    public bool TryDeserializeFromFile(string filePath, out T? data)
    {
        data = null;

        if (!File.Exists(filePath))
            return false;
        
        var jsonStr = File.ReadAllText(filePath);

        if (JsonConvert.DeserializeObject<T>(jsonStr) is { } deserializedData)
            data = deserializedData;

        return data != null;
    }

    public bool TrySerializeToFile(string filePath, T data)
    {
        if (JsonConvert.SerializeObject(data, Formatting.Indented) is not { } jsonStr)
            return false;

        var directory = Path.GetDirectoryName(filePath);
        if (string.IsNullOrEmpty(directory))
            return false;
        
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        File.WriteAllTextAsync(filePath, jsonStr);
        return true;
    }
}
