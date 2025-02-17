using RacingAidWpf.Core.Configuration;
using RacingAidWpf.Core.Overlays;

namespace RacingAidWpf.Core.Telemetry;

public class TelemetryConfigSection(IConfig config) : OverlayConfigSection(config, "Telemetry")
{
    public bool UseMetricUnits
    {
        get => GetBool(nameof(UseMetricUnits));
        set => SetValue(nameof(UseMetricUnits), value.ToString());
    }
}