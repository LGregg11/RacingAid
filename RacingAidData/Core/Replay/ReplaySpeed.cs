using System.ComponentModel;

namespace RacingAidData.Core.Replay;

public enum ReplaySpeed
{
    [Description("Normal")]
    X1,
    
    [Description("2x")]
    X2,
    
    [Description("4x")]
    X4,
    
    [Description("8x")]
    X8,
    
    [Description("16x")]
    X16
}