namespace RacingAidWpf.Configuration;

public abstract class ConfigSection(IConfig config, string section)
{
    protected T? GetValue<T>(string key, T defaultValue)
    {
        if (!config.TryGetValue(section, key, out T? value))
        {
            SetValue(key, defaultValue);
            value = defaultValue;
        }

        return value;
    }

    protected bool SetValue<T>(string key, T? value)
    {
        return config.SetValue(section, key, value);
    }
}