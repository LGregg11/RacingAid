using RacingAidWpf.Tracks;

namespace RacingAidWpf.Configuration;

public class TrackMapConfigSection(IConfig config) : ConfigSection(config, "TrackMap")
{
    public DriverNumberType DriverNumberType
    {
        get => GetEnum(nameof(DriverNumberType), DriverNumberType.OverallPosition);
        set => SetValue(nameof(DriverNumberType), Enum.GetName(typeof(DriverNumberType), value));
    }
}
