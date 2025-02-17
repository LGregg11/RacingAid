using RacingAidData.Core.Models;

namespace RacingAidWpf.Core.Telemetry;

public class TelemetryInfo
{
    public float ThrottlePercentage { get; private set; }
    public float BrakePercentage { get; private set; }
    public float ClutchPercentage { get; private set; }
    public float SpeedMetresPerSecond { get; private set; }
    public int Gear { get; private set; }
    public float SteeringAngleDegrees { get; private set; }
    
    public void UpdateFromData(TelemetryModel telemetry)
    {
        ThrottlePercentage = telemetry.ThrottleInput;
        BrakePercentage = telemetry.BrakeInput;
        ClutchPercentage = 1f - telemetry.ClutchInput;
        SpeedMetresPerSecond = telemetry.SpeedMs;
        SteeringAngleDegrees = telemetry.SteeringAngleDegrees;
        Gear = telemetry.Gear;
    }

    public void Clear()
    {
        ThrottlePercentage = 0f;
        BrakePercentage = 0f;
        ClutchPercentage = 0f;
        SpeedMetresPerSecond = 0f;
        Gear = 0;
        SteeringAngleDegrees = 0f;
    }
}