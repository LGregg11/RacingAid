using System.Reflection;

namespace RacingAidWpf.Resources;

public static class Resource
{
    private static readonly string AssemblyPath =
        $"pack://application:,,,/{ExecutingAssembly.GetName().Name};component/Resources";
    
    public static Assembly ExecutingAssembly => Assembly.GetExecutingAssembly();
    
    public static Uri SteeringWheelUri => GetImageUri("SteeringWheel.png");

    public static Uri GetImageUri(string imageName) => new($"{AssemblyPath}/Images/{imageName}");
}