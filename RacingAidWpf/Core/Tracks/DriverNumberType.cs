using System.ComponentModel;

namespace RacingAidWpf.Core.Tracks;

public enum DriverNumberType
{
    [Description("Overall position")]
    OverallPosition,
    
    [Description("Class position")]
    ClassPosition,
    
    [Description("Car number")]
    CarNumber
}