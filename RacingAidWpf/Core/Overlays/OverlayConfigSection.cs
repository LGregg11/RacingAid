using RacingAidWpf.Core.Configuration;

namespace RacingAidWpf.Core.Overlays;

public class OverlayConfigSection(IConfig config, string section) : ConfigSection(config, section)
{
    public bool IsEnabled
    {
        get => GetBool(nameof(IsEnabled), false);
        set => SetValue(nameof(IsEnabled), value.ToString());
    }
}