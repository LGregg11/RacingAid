using System.IO;
using System.Reflection;

namespace RacingAidWpf.Resources;

public static class Resource
{
    public static string ExecutingDirectory =>
        Path.GetDirectoryName(ExecutingAssembly.Location) ?? Directory.GetCurrentDirectory();
    
    public static Uri SteeringWheelUri => GetImageUri("SteeringWheel.png");
    
    private static readonly string AssemblyPath =
        $"pack://application:,,,/{ExecutingAssembly.GetName().Name};component/Resources";

    private static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();

    private static Uri GetImageUri(string imageName) => new($"{AssemblyPath}/Images/{imageName}");
}