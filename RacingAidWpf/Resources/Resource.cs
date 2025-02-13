﻿using System.IO;
using System.Reflection;

namespace RacingAidWpf.Resources;

public static class Resource
{
    private const string ApplicationDirectoryName = "RacingAid";
    private const string ConfigDirectoryName = "Config";
    public static string ConfigDirectory
    {
        get
        {
            var configDirectory = Path.Combine(AppDataDirectory, ApplicationDirectoryName, ConfigDirectoryName);
            
            if (!Directory.Exists(configDirectory))
                Directory.CreateDirectory(configDirectory);
            
            return configDirectory;
        }
    }
    
    private const string DataDirectoryName = "Data";
    public static string DataDirectory
    {
        get
        {
            var dataDirectory = Path.Combine(AppDataDirectory, ApplicationDirectoryName, DataDirectoryName);
            
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);
            
            return dataDirectory;
        }
    }
    
    
    public static Uri SteeringWheelUri => GetImageUri("SteeringWheel.png");
    
    private static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
    private static string AssemblyPath => $"pack://application:,,,/{ExecutingAssembly.GetName().Name};component/Resources";
    private static string AppDataDirectory => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private static Uri GetImageUri(string imageName) => new($"{AssemblyPath}/Images/{imageName}");
}