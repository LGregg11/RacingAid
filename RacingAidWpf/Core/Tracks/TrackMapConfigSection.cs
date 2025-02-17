using RacingAidWpf.Core.Configuration;
using RacingAidWpf.Core.Overlays;

namespace RacingAidWpf.Core.Tracks;

public class TrackMapConfigSection(IConfig config) : OverlayConfigSection(config, "TrackMap")
{
    public DriverNumberType DriverNumberType
    {
        get => GetEnum(nameof(DriverNumberType), DriverNumberType.OverallPosition);
        set => SetValue(nameof(DriverNumberType), Enum.GetName(typeof(DriverNumberType), value));
    }
}
