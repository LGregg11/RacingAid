using System.IO;
using RacingAidWpf.Resources;

namespace RacingAidWpf.Configuration;

public static class ConfigSectionSingleton
{
    private static readonly string ConfigFilePath = Path.Combine(Resource.DataDirectory, "config.ini");
    
    private static Config config;
    private static Config Config => config ??= new Config(ConfigFilePath);
    
    private static GeneralConfigSection generalSection;
    public static GeneralConfigSection GeneralSection =>
        generalSection ??= new GeneralConfigSection(Config);

    private static TimesheetConfigSection timesheetSection;
    public static TimesheetConfigSection TimesheetSection =>
        timesheetSection ??= new TimesheetConfigSection(Config);

    private static RelativeConfigSection relativeSection;
    public static RelativeConfigSection RelativeSection =>
        relativeSection ??= new RelativeConfigSection(Config);
    
    private static TelemetryConfigSection telemetrySection;
    public static TelemetryConfigSection TelemetrySection =>
        telemetrySection ??= new TelemetryConfigSection(Config);
}