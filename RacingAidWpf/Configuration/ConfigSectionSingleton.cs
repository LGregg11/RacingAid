using System.IO;
using RacingAidWpf.Resources;
using RacingAidWpf.Timesheets.Leaderboard;
using RacingAidWpf.Timesheets.Relative;
using RacingAidWpf.Tracks;

namespace RacingAidWpf.Configuration;

public static class ConfigSectionSingleton
{
    private static readonly string ConfigFilePath = Path.Combine(Resource.ConfigDirectory, "config.ini");
    
    private static Config config;
    private static Config Config => config ??= new Config(ConfigFilePath);
    
    private static GeneralConfigSection generalSection;
    public static GeneralConfigSection GeneralSection =>
        generalSection ??= new GeneralConfigSection(Config);

    private static LeaderboardConfigSection leaderboardSection;
    public static LeaderboardConfigSection LeaderboardSection =>
        leaderboardSection ??= new LeaderboardConfigSection(Config);

    private static RelativeConfigSection relativeSection;
    public static RelativeConfigSection RelativeSection =>
        relativeSection ??= new RelativeConfigSection(Config);
    
    private static TelemetryConfigSection telemetrySection;
    public static TelemetryConfigSection TelemetrySection =>
        telemetrySection ??= new TelemetryConfigSection(Config);
    
    private static TrackMapConfigSection trackMapSection;
    public static TrackMapConfigSection TrackMapSection =>
        trackMapSection ??= new TrackMapConfigSection(Config);
}