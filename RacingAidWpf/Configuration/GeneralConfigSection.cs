namespace RacingAidWpf.Configuration;

public class GeneralConfigSection(IConfig config) : ConfigSection(config, "General")
{
    public int PrimaryMonitor
    {
        get => GetInt(nameof(PrimaryMonitor), 1);
        set => SetValue(nameof(PrimaryMonitor), value.ToString());
    }
    
    public int RefreshRateMs
    {
        get => GetInt(nameof(RefreshRateMs), 33);
        set => SetValue(nameof(RefreshRateMs), value.ToString());
    }
}