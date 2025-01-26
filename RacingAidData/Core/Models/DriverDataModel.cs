namespace RacingAidData.Core.Models;

public class DriverDataModel : RaceDataModel
{
    /// <summary>
    /// The speed of the driver in metres per second
    /// </summary>
    public float SpeedMs { get; set; }
    
    /// <summary>
    /// The forward direction of the driver's car in degrees (North = 0, West = 270)
    /// </summary>
    public float ForwardDirectionDeg { get; set; }
    
    /// <summary>
    /// The number of laps driven (float to keep track of current lap progress)
    /// </summary>
    public float LapsDriven { get; set; }
}