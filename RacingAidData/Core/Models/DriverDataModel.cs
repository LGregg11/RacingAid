namespace RacingAidData.Core.Models;

public class DriverDataModel : RaceDataModel
{
    /// <summary>
    /// The velocity of the driver in metres per second
    /// </summary>
    public Velocity? VelocityMs { get; init; }
    
    /// <summary>
    /// The forward direction of the driver's car in degrees (North = 0, West = 270)
    /// </summary>
    public float ForwardDirectionDeg { get; init; }
    
    /// <summary>
    /// The number of laps driven (float to keep track of current lap progress)
    /// </summary>
    public float LapsDriven { get; init; }
    
    /// <summary>
    /// Whether the driver is in the pits
    /// </summary>
    public bool InPits { get; init; }
    
    /// <summary>
    /// The number of incidents/penalties that the driver has accumulated
    /// </summary>
    public int Incidents { get; init; }
}

public class Velocity(float x, float y)
{
    /// <summary>
    /// +ve X = forward
    /// </summary>
    public float X { get; set; } = x;
    
    /// <summary>
    /// +ve Y = left
    /// </summary>
    public float Y { get; set; } = y;
}