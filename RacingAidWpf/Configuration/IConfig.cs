namespace RacingAidWpf.Configuration;

public interface IConfig
{
    public bool TryGetValue<T>(string section, string key, out T? value);
    public bool SetValue<T>(string section, string key, T value);
}