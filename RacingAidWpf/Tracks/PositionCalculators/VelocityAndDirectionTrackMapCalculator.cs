using RacingAidData.Core.Models;

namespace RacingAidWpf.Tracks.PositionCalculators;

public class VelocityAndDirectionTrackMapCalculator : TrackMapPositionCalculator
{
    private const float DegToRad = MathF.PI / 180f;
    private const float OneThousand = 1000f;
    
    public override TrackMapPosition CalculatePosition(TrackMapPosition previousPosition, DriverDataModel driverDataModel, float timeDeltaMs)
    {
        // NOTE: Ms for speed = metres per second, Ms time = milliseconds.. Sorry its a bit awkward

        if (driverDataModel.VelocityMs == null)
            throw new ArgumentNullException($"{nameof(driverDataModel.VelocityMs)} is null");
        
        // For the sake of formulas, lets just call the forward angle THETA and pitch angle PHI
        var timeDeltaS = timeDeltaMs / OneThousand;
        var forwardRad = driverDataModel.ForwardDirectionDeg * DegToRad;
        var pitchRad = driverDataModel.PitchDirectionDeg * DegToRad;

        var cosTheta = MathF.Cos(forwardRad);
        var sinTheta = MathF.Sin(forwardRad);
        var cosPhi = MathF.Cos(pitchRad);
        var sinPhi = MathF.Sin(pitchRad);
        
        var forwardSpeedMs = driverDataModel.VelocityMs.X; // Yes, relative x is forward direction
        var leftSpeedMs = driverDataModel.VelocityMs.Y;
        var upwardSpeedMs = driverDataModel.VelocityMs.Z;

        var forwardSpeedSinTheta = forwardSpeedMs * sinTheta;
        var forwardSpeedCosTheta = forwardSpeedMs * cosTheta;
        var leftSpeedSinTheta = leftSpeedMs * sinTheta;
        var leftSpeedCosTheta = leftSpeedMs * cosTheta;
        var upwardSpeedSinPhi = upwardSpeedMs * sinPhi;
        var upwardSpeedCosPhi = upwardSpeedMs * cosPhi;
        
        // x = f_x * Sin(theta) - f_y * Cos(theta)
        // y = f_x * Cos(theta) * Cos(phi) + f_y * Sin(theta) * Cos(phi) - f_z Sin(phi)
        // z = f_x * Cos(theta) * Sin(phi) + f_y * Sin(theta) * Sin(phi) + f_z * Cos(phi)
        var xSpeedMs = forwardSpeedSinTheta - leftSpeedCosTheta;
        var ySpeedMs = (forwardSpeedCosTheta * cosPhi) + (leftSpeedSinTheta * cosPhi) - upwardSpeedSinPhi;
        var zSpeedMs = (forwardSpeedCosTheta * sinPhi) + (leftSpeedSinTheta * sinPhi) + upwardSpeedCosPhi;
        
        // Use euler-esque approximations to determine the driver's current whereabouts..
        var approxDistanceXMetres = timeDeltaS * xSpeedMs;
        var approxDistanceYMetres = timeDeltaS * ySpeedMs;
        var approxDistanceZMetres = timeDeltaS * zSpeedMs;
        
        var approximateNewX = previousPosition.X + approxDistanceXMetres;
        var approximateNewY = previousPosition.Y + approxDistanceYMetres;
        var approximateNewZ = previousPosition.Z + approxDistanceZMetres;
        return new TrackMapPosition(driverDataModel.LapDistanceMetres, approximateNewX, approximateNewY, approximateNewZ);
    }
}