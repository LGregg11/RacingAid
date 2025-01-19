using System.IO;
using System.Windows;
using Salaros.Configuration;

namespace RacingAidWpf.Configuration;

public static class ConfigSingleton
{
    private static readonly string ConfigFilePath = Path.Combine(Application.ResourceAssembly.Location, "config.ini");
    
    private static ConfigParser? configParser;
    public static ConfigParser ConfigParser => configParser ??= new ConfigParser(ConfigFilePath);
}