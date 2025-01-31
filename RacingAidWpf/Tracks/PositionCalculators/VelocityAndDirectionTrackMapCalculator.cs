using MathNet.Numerics.LinearAlgebra;
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
        
        // MATRICES
        var xAxisRotationMatrix = Matrix<float>.Build.DenseOfArray(new [,]
        {
            { 1f , 0f, 0f },
            { 0f, cosPhi, -1f * sinPhi },
            { 0f, sinPhi, cosPhi }
        });
        var zAxisRotationMatrix = Matrix<float>.Build.DenseOfArray(new [,]
        {
            { cosTheta, -1f * sinTheta, 0f },
            { sinTheta, cosTheta, 0f },
            { 0f, 0f, 1f }
        });

        var combinedMatrix = xAxisRotationMatrix * zAxisRotationMatrix;
        var velocityMatrix = Vector<float>.Build.Dense([forwardSpeedMs, leftSpeedMs, upwardSpeedMs]);
        var rotatedVelocityMatrix = combinedMatrix * velocityMatrix;
        
        // Use euler-esque approximations to determine the driver's current whereabouts..
        var approxDistanceXMetres = timeDeltaS * rotatedVelocityMatrix[0];
        var approxDistanceYMetres = timeDeltaS * rotatedVelocityMatrix[1];
        var approxDistanceZMetres = timeDeltaS * rotatedVelocityMatrix[2];
        
        var approximateNewX = previousPosition.X + approxDistanceXMetres;
        var approximateNewY = previousPosition.Y + approxDistanceYMetres;
        var approximateNewZ = previousPosition.Z + approxDistanceZMetres;
        return new TrackMapPosition(driverDataModel.LapDistanceMetres, approximateNewX, approximateNewY, approximateNewZ);
    }
}