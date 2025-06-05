namespace RacingAidWpf.Core.Configuration;

public class GeneralConfigSection(IConfig config) : ConfigSection(config, "General")
{
    public int UpdateIntervalMs
    {
        get => GetInt(nameof(UpdateIntervalMs), 33);
        set => SetValue(nameof(UpdateIntervalMs), value.ToString());
    }

    /// <summary>
    /// A get-only property for the dev mode (get only so it's hidden and only settable in the config)
    /// </summary>
    public bool EnabledDevMode => GetBool(nameof(EnabledDevMode), false);
}