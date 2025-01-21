namespace RacingAidWpf.Configuration;

public class TelemetryConfigSection(IConfig config) : ConfigSection(config, "Telemetry")
{
    public bool UseMetricUnits
    {
        get => GetBool(nameof(UseMetricUnits));
        set => SetValue(nameof(UseMetricUnits), value.ToString());
    }
}