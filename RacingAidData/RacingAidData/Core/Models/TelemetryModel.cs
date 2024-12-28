namespace RacingAidData.Core.Models;

/// <summary>
/// A Standard telemetry data model
/// </summary>
public struct TelemetryModel
{
    public float SpeedMetresPerSecond { get; init; }
    
    public float ThrottlePercentage { get; init; }
    
    public float BrakePercentage { get; init; }
    
    /// <summary>
    /// The steering angle in degrees. Negative steering angle = counter-clockwise rotation
    /// </summary>
    public float SteeringAngleDegrees { get; init; }
    
    public float Rpm { get; init; }
    
    
    /// <summary>
    /// The gear. 0 = Neutral, -1 = Reverse
    /// </summary>
    public short Gear { get; init; }
}