using RacingAidWpf.Configuration;
using RacingAidWpf.Overlays;

namespace RacingAidWpf.Telemetry;

public class TelemetryConfigSection(IConfig config) : OverlayConfigSection(config, "Telemetry")
{
    public bool UseMetricUnits
    {
        get => GetBool(nameof(UseMetricUnits));
        set => SetValue(nameof(UseMetricUnits), value.ToString());
    }
}