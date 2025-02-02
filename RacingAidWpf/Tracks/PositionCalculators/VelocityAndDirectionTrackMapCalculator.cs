using MathNet.Numerics.LinearAlgebra;
using RacingAidData.Core.Models;

namespace RacingAidWpf.Tracks.PositionCalculators;

public class VelocityAndDirectionTrackMapCalculator : TrackMapPositionCalculator
{
    private const float DegToRad = MathF.PI / 180f;
    
    public override TrackMapPosition CalculatePosition(TrackMapPosition previousPosition, DriverDataModel driverDataModel, float timeDeltaMs)
    {
        // TODO: Confirm the Z calculations are as accurate as they can be
        // NOTE: Ms for speed = metres per second, Ms time = milliseconds.. Sorry its a bit awkward

        if (driverDataModel.VelocityMs == null)
            throw new ArgumentNullException($"{nameof(driverDataModel.VelocityMs)} is null");
        
        var timeDeltaS = timeDeltaMs / OneThousand;
        
        var vF = driverDataModel.VelocityMs.X;
        var vL = driverDataModel.VelocityMs.Y;
        var vU = driverDataModel.VelocityMs.Z;
        
        var yawNorthDeg = driverDataModel.ForwardDirectionDeg;
        var pitchDeg = driverDataModel.PitchDeg;
        var rollDeg = driverDataModel.RollDeg;
        
        var yawNorth = yawNorthDeg * DegToRad;
        var pitch = pitchDeg * DegToRad;
        var roll = rollDeg * DegToRad;
        
        var cosYaw = MathF.Cos(yawNorth);
        var sinYaw = MathF.Sin(yawNorth);
        var cosPitch = MathF.Cos(pitch);
        var sinPitch = MathF.Sin(pitch);
        var cosRoll = MathF.Cos(roll);
        var sinRoll = MathF.Sin(roll);
        
        var vFx = vF * cosPitch * sinYaw;
        var vFy = vF * cosPitch * cosYaw;
        var vFz = vF * sinPitch;

        var vLx =  vL * (cosRoll * cosYaw - sinRoll * sinPitch * sinYaw);
        var vLy =  vL * (cosRoll * sinYaw + sinRoll * sinPitch * cosYaw);
        var vLz = -1f * vL * sinRoll * cosPitch;

        var vUx = -1f * vU * (sinRoll * cosYaw + cosRoll * sinPitch * sinYaw);
        var vUy = -1f * vU * (sinRoll * sinYaw - cosRoll * sinPitch * cosYaw);
        var vUz = vU * cosRoll * cosPitch;

        var vX = vFx + vLx + vUx;
        var vY = vFy + vLy + vUy;
        var vZ = vFz + vLz + vUz;
        
        // Use euler-esque approximations to determine the driver's current whereabouts..
        var approxDistanceXMetres = timeDeltaS * vX;
        var approxDistanceYMetres = timeDeltaS * vY;
        var approxDistanceZMetres = timeDeltaS * vZ;
        
        var approximateNewX = previousPosition.X + approxDistanceXMetres;
        var approximateNewY = previousPosition.Y + approxDistanceYMetres;
        var approximateNewZ = previousPosition.Z + approxDistanceZMetres;
        return new TrackMapPosition(driverDataModel.LapDistanceMetres, approximateNewX, approximateNewY, approximateNewZ);
    }
}