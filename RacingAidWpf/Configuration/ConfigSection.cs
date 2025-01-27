using System.Globalization;

namespace RacingAidWpf.Configuration;

public class ConfigSection(IConfig config, string section)
{
    public event Action ConfigUpdated;
    
    protected bool GetBool(string key, bool defaultValue = true)
    {
        if (!config.TryGetBool(section, key, out var configValue))
            SetValue(key, defaultValue.ToString());
        
        var value = configValue ?? defaultValue;
        return value;
    }
    
    protected int GetInt(string key, int defaultValue = 0)
    {
        if (!config.TryGetInt(section, key, out var configValue))
            SetValue(key, defaultValue.ToString());
        
        return configValue ?? defaultValue;
    }
    
    protected T GetEnum<T>(string key, T defaultValue) where T : struct, Enum
    {
        var defaultValueStr = Enum.GetName(typeof(T), defaultValue);
        
        if (!config.TryGetString(section, key, out var configValue))
            SetValue(key, defaultValueStr);

        return Enum.TryParse(configValue, out T parseValue) ? parseValue : defaultValue;
    }
    
    protected double GetDouble(string key, double defaultValue = 0d)
    {
        if (!config.TryGetDouble(section, key, out var configValue))
            SetValue(key, defaultValue.ToString(CultureInfo.InvariantCulture));
        
        return configValue ?? defaultValue;
    }
    
    protected string GetString(string key, string defaultValue = "")
    {
        if (!config.TryGetString(section, key, out var configValue))
            SetValue(key, defaultValue);
        
        return configValue ?? defaultValue;
    }

    protected void SetValue(string key, string value)
    {
        config.SetValue(section, key, value);
        ConfigUpdated?.Invoke();
    }
}