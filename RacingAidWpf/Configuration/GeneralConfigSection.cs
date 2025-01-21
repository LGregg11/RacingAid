namespace RacingAidWpf.Configuration;

public class GeneralConfigSection(IConfig config) : ConfigSection(config, "General")
{
    public int PrimaryScreen
    {
        get => GetInt(nameof(PrimaryScreen), 1);
        set => SetValue(nameof(PrimaryScreen), value.ToString());
    }
    
    public int UpdateIntervalMs
    {
        get => GetInt(nameof(UpdateIntervalMs), 33);
        set => SetValue(nameof(UpdateIntervalMs), value.ToString());
    }
}