using System.ComponentModel;

namespace RacingAidWpf.Tracks;

public enum DriverNumberType
{
    [Description("Overall position")]
    OverallPosition,
    
    [Description("Class position")]
    ClassPosition,
    
    [Description("Car number")]
    CarNumber
}