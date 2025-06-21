using System.Numerics;
using RacingAidData.Core.Models;

namespace RacingAidWpf.Core.Tracks.PositionCalculators;

public class VelocityAndDirectionTrackMapCalculator : TrackMapPositionCalculator
{
    private const float DegToRad = MathF.PI / 180f;
    
    public override TrackMapPosition CalculatePosition(TrackMapPosition previousPosition, DriverDataModel driverDataModel, float timeDeltaMs)
    {
        // NOTE: Ms for speed = metres per second, Ms time = milliseconds.. Sorry its a bit awkward

        if (driverDataModel.VelocityMs == null)
            throw new ArgumentNullException(
                $"{nameof(driverDataModel)}'s {nameof(driverDataModel.VelocityMs)} is null");
        
        var driverVelocityMs = driverDataModel.VelocityMs;
        var driverVelocityVectorMs = new Vector3(driverVelocityMs.X, driverVelocityMs.Y, driverVelocityMs.Z);
        
        var yawNorth = (90f - driverDataModel.ForwardDirectionDeg) * DegToRad;
        var pitch = -1f * driverDataModel.PitchDeg * DegToRad;
        var roll = driverDataModel.RollDeg * DegToRad;

        var rollRotation = Matrix4x4.CreateRotationX(roll);
        var pitchRotation = Matrix4x4.CreateRotationY(pitch);
        var yawRotation = Matrix4x4.CreateRotationZ(yawNorth);

        var localToWorldRotationMatrix = rollRotation * pitchRotation * yawRotation;
        
        var velocityVectorMs = Vector3.Transform(driverVelocityVectorMs, localToWorldRotationMatrix);
        var timeDeltaS = timeDeltaMs / OneThousand;
        
        // Use euler-esque approximations to determine the driver's current whereabouts..
        var approxDistanceVectorMetres = velocityVectorMs * timeDeltaS;
        
        var approximateNewX = previousPosition.X + approxDistanceVectorMetres.X;
        var approximateNewY = previousPosition.Y + approxDistanceVectorMetres.Y;
        var approximateNewZ = previousPosition.Z + approxDistanceVectorMetres.Z;
        
        return new TrackMapPosition(driverDataModel.LapDistanceMetres, approximateNewX, approximateNewY, approximateNewZ);
    }
}