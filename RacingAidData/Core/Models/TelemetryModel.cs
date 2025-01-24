namespace RacingAidData.Core.Models;

/// <summary>
/// A Standard telemetry data model
/// </summary>
public sealed class TelemetryModel : RaceDataModel
{
    public float SpeedMs { get; init; }
    
    /// <remarks>
    /// 1 = 100%
    /// </remarks>
    public float ThrottleInput { get; init; }
    
    /// <remarks>
    /// 1 = 100%
    /// </remarks>
    public float BrakeInput { get; init; }

    /// <remarks>
    /// 1 = 100% engaged (i.e. released)
    /// </remarks>
    public float ClutchInput { get; init; }
    
    /// <summary>
    /// The steering angle in degrees. Negative steering angle = counter-clockwise rotation
    /// </summary>
    public float SteeringAngleDegrees { get; init; }
    
    public float Rpm { get; init; }
    
    
    /// <summary>
    /// The gear. 0 = Neutral, -1 = Reverse
    /// </summary>
    public int Gear { get; init; }
}