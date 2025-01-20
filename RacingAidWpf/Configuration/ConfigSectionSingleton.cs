using System.IO;
using System.Windows;

namespace RacingAidWpf.Configuration;

public static class ConfigSectionSingleton
{
    private static readonly string ConfigFilePath = Path.Combine(Path.GetDirectoryName(Application.ResourceAssembly.Location) ?? string.Empty, "config.ini");
    private static Config Config => new(ConfigFilePath);

    private static TimesheetConfigSection? timesheetSection;
    public static TimesheetConfigSection TimesheetSection =>
        timesheetSection ??= new TimesheetConfigSection(Config);
}