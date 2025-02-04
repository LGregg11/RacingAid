using System.IO;
using Salaros.Configuration;

namespace RacingAidWpf.Configuration;

public class Config : IConfig
{
    private readonly ConfigParser configParser;

    public Config(string configFilePath)
    {
        if (string.IsNullOrEmpty(configFilePath))
            throw new NullReferenceException(nameof(configFilePath));

        if (!File.Exists(configFilePath))
        {
            // Create but immediately close
            using (File.Create(configFilePath)) { }
        }

        configParser = new ConfigParser(configFilePath);
    }

    public bool TryGetBool(string section, string key, out bool? value)
    {
        value = null;
        if (TryGetString(section, key, out var strValue) && bool.TryParse(strValue, out var boolValue))
            value = boolValue;

        return value != null;
    }

    public bool TryGetInt(string section, string key, out int? value)
    {
        value = null;
        if (TryGetString(section, key, out var strValue) && int.TryParse(strValue, out var intValue))
            value = intValue;

        return value.HasValue;
    }

    public bool TryGetDouble(string section, string key, out double? value)
    {
        value = null;
        if (TryGetString(section, key, out var strValue) && double.TryParse(strValue, out var doubleValue))
            value = doubleValue;

        return value.HasValue;
    }

    public bool TryGetString(string section, string key, out string value)
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