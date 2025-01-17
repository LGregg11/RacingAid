using IRSDKSharper;
using RacingAidData.Core.Deserializers;
using RacingAidData.Core.Models;

namespace RacingAidData.Simulators.iRacing;

public class iRacingDataDeserializer : IDeserializeData
{
    private static float RadToDeg => 180f / MathF.PI;
    
    public bool TryDeserializeData(object data, out List<RaceDataModel> models)
    {
        models = [];
        
        if (data is not IRacingSdkData iRacingData)
            return false;

        // Driver data
        if (iRacingData.SessionInfo?.DriverInfo != null)
        {
            IRacingSdkSessionInfo.DriverInfoModel driverInfoModel = iRacingData.SessionInfo.DriverInfo;
        
            var drivers = CreateDrivers(driverInfoModel);
            DriversModel driversModel = new DriversModel
            {
                Drivers = drivers,
                LocalDriver = drivers[driverInfoModel.DriverCarIdx]
            };
        
            models.Add(driversModel);
        }
        
        // Telemetry data
        // Try get from TelemetryDataProperties
        models.Add(CreateTelemetryModel(iRacingData));

        return true;
    }

    private static List<DriverModel> CreateDrivers(IRacingSdkSessionInfo.DriverInfoModel driverInfoModel)
    {
        var drivers = new List<DriverModel>();
        foreach (var driverData in driverInfoModel.Drivers)
        {
            if (!int.TryParse(driverData.CarNumber, out var carNumber))
                carNumber = -1;
            
            drivers.Add(new DriverModel
            {
                FullName = driverData.UserName,
                CarModel = driverData.CarScreenName,
                CarNumber = carNumber, 
                SkillRating = driverData.IRating.ToString(),
                SafetyRating = driverData.LicString // TODO: might be wrong tbf..
            });
        }

        return drivers;
    }

    private static RaceDataModel CreateTelemetryModel(IRacingSdkData iRacingData)
    {
        return new TelemetryModel
        {
            BrakePercentage = iRacingData.GetFloat("Brake"),
            Gear = iRacingData.GetInt("Gear"),
            Rpm = iRacingData.GetFloat("RPM"),
            SpeedMetresPerSecond = iRacingData.GetFloat("Speed"),
            SteeringAngleDegrees = iRacingData.GetFloat("SteeringWheelAngle") * RadToDeg,
            ThrottlePercentage = iRacingData.GetFloat("Throttle")
        };
    }
}