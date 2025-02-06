using RacingAidWpf.Configuration;
using RacingAidWpf.Overlays;

namespace RacingAidWpf.Tracks;

public class TrackMapConfigSection(IConfig config) : OverlayConfigSection(config, "TrackMap")
{
    public DriverNumberType DriverNumberType
    {
        get => GetEnum(nameof(DriverNumberType), DriverNumberType.OverallPosition);
        set => SetValue(nameof(DriverNumberType), Enum.GetName(typeof(DriverNumberType), value));
    }
}
