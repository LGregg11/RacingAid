using RacingAidData.Core.Models;

namespace RacingAidWpf.Tracks.PositionCalculators;

public class VelocityAndDirectionTrackMapCalculator : TrackMapPositionCalculator
{
    private const float DegToRad = MathF.PI / 180f;
    private const float OneThousand = 1000f;
    
    public override TrackMapPosition CalculatePosition(TrackMapPosition previousPosition, DriverDataModel driverDataModel, float timeDeltaMs)
    {
        // NOTE: Ms for speed = metres per second, Ms time = milliseconds.. Sorry its a bit awkward
        
        var timeDeltaS = timeDeltaMs / OneThousand;
        var forwardRad = driverDataModel.ForwardDirectionDeg * DegToRad;
        var forwardSpeedMs = driverDataModel.VelocityMs.X; // Yes, relative x is forward direction
        var leftSpeedMs = driverDataModel.VelocityMs.Y;

        var forwardSpeedSinNorth = forwardSpeedMs * MathF.Sin(forwardRad);
        var forwardSpeedCosNorth = forwardSpeedMs * MathF.Cos(forwardRad);
        var leftSpeedSinNorth = leftSpeedMs * MathF.Sin(forwardRad);
        var leftSpeedCosNorth = leftSpeedMs * MathF.Cos(forwardRad);
        
        // y = f_x * Cos(theta) + f_y * Sin(theta), x = f_x * Sin(theta) - f_y * Cos(theta)
        var northSpeedMs = forwardSpeedCosNorth + leftSpeedSinNorth;
        var eastSpeedMs = forwardSpeedSinNorth - leftSpeedCosNorth;
        
        // Use euler-esque approximations to determine the driver's current whereabouts..
        var approxDistanceXMetres = timeDeltaS * eastSpeedMs;
        var approxDistanceYMetres = timeDeltaS * northSpeedMs;
        
        var approximateNewX = previousPosition.X + approxDistanceXMetres;
        var approximateNewY = previousPosition.Y + approxDistanceYMetres;
        return new TrackMapPosition(approximateNewX, approximateNewY);
    }
}