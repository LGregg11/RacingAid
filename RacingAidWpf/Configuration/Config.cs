using Salaros.Configuration;

namespace RacingAidWpf.Configuration;

public class Config(string path) : IConfig
{
    private readonly ConfigParser configParser = new(path);

    public bool TryGetValue<T>(string section, string key, out T? value)
    {
        value = default;
        if (configParser.GetValue(section, key) is not T configValue)
            return false;
        
        value = configValue;
        return true;
    }

    public bool SetValue<T>(string section, string key, T value) =>
        value?.ToString() is { } stringValue && configParser.SetValue(section, key, stringValue);
}