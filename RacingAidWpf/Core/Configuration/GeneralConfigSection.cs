namespace RacingAidWpf.Core.Configuration;

public class GeneralConfigSection(IConfig config) : ConfigSection(config, "General")
{
    public int UpdateIntervalMs
    {
        get => GetInt(nameof(UpdateIntervalMs), 33);
        set => SetValue(nameof(UpdateIntervalMs), value.ToString());
    }
}