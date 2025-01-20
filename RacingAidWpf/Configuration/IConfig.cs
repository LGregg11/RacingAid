namespace RacingAidWpf.Configuration;

public interface IConfig
{
    public bool TryGetBool(string section, string key, out bool? value);
    public bool TryGetInt(string section, string key, out int? value);
    public bool TryGetDouble(string section, string key, out double? value);
    public bool TryGetString(string section, string key, out string? value);
    public bool SetValue(string section, string key, string value);
}