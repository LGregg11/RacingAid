using System.ComponentModel;

namespace RacingAidData.Simulators;

public enum Simulator
{
    [Description("iRacing")]
    iRacing,
    
    [Description("Formula 1")]
    F1,
    
    #if DEBUG
    [Description("Data Faker")]
    DataFaker,
    #endif
}