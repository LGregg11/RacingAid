using RacingAidWpf.Configuration;

namespace RacingAidWpf.Overlays;

public class OverlayConfigSection(IConfig config, string section) : ConfigSection(config, section)
{
    public bool IsEnabled
    {
        get => GetBool(nameof(IsEnabled), false);
        set => SetValue(nameof(IsEnabled), value.ToString());
    }
}