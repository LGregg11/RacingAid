using System.IO;
using System.Windows;

namespace RacingAidWpf.Configuration;

public static class ConfigSingleton
{
    private static readonly string ConfigFilePath = Path.Combine(Application.ResourceAssembly.Location, "config.ini");
    
    private static Config? config;
    public static Config Config => config ??= new Config(ConfigFilePath);
}