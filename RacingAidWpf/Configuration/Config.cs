using System.ComponentModel;
using System.IO;
using Salaros.Configuration;

namespace RacingAidWpf.Configuration;

public class Config : IConfig
{
    private readonly ConfigParser configParser;

    public Config(string configFilePath)
    {
        if (!File.Exists(configFilePath))
            File.Create(configFilePath);

        configParser = new ConfigParser(configFilePath);
    }

    public bool TryGetBool(string section, string key, out bool? value)
    {
        value = null;
        if (configParser.GetValue(section, key, true) is { } configValue)
            value = configValue;

        return value != null;
    }

    public bool TryGetInt(string section, string key, out int? value)
    {
        value = null;
        if (configParser.GetValue(section, key, 0) is { } configValue)
            value = configValue;

        return value.HasValue;
    }

    public bool TryGetDouble(string section, string key, out double? value)
    {
        value = null;
        if (configParser.GetValue(section, key, 0d) is { } configValue)
            value = configValue;

        return value.HasValue;
    }

    public bool TryGetString(string section, string key, out string? value)
    {
        value = null;
        if (configParser.GetValue(section, key) is { } configValue)
            value = configValue;

        return value != null;
    }

    public bool SetValue(string section, string key, string value)
    {
        configParser.SetValue(section, key, value);
        configParser.Save();

        return false;
    }
}