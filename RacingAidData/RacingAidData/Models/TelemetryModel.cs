namespace RacingAidData.Models;

/// <summary>
/// A Standard telemetry data model
/// </summary>
public struct TelemetryModel
{
    public float SpeedMetresPerSecond { get; private set; }
    
    public float ThrottlePercentage { get; private set; }
    
    public float BrakePercentage { get; private set; }
    
    public float SteeringAngle { get; private set; }
    
    public float Rpm { get; private set; }
    
    public short Gear { get; private set; }
}