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

        // Timesheet data
        if (iRacingData.SessionInfo?.DriverInfo is {} driverInfo &&
            iRacingData.SessionInfo.SessionInfo.Sessions.LastOrDefault() is {} latestSession)
        {
            var entries = CreateTimesheetEntries(driverInfo, latestSession);
            TimesheetModel timesheetModel = new TimesheetModel
            {
                Entries = entries,
                LocalEntry = entries.FirstOrDefault(e => e.IsLocal)
            };
        
            models.Add(timesheetModel);
        }
        
        // Telemetry data
        // Try get from TelemetryDataProperties
        models.Add(CreateTelemetryModel(iRacingData));

        return true;
    }

    private static List<TimesheetEntryModel> CreateTimesheetEntries(IRacingSdkSessionInfo.DriverInfoModel driverInfo,
        IRacingSdkSessionInfo.SessionInfoModel.SessionModel latestSession)
    {
        var timesheetEntries = new List<TimesheetEntryModel>();
        
        // Loop through position results to ensure the position order is correct - can grab necessary driver info
        foreach (var positionResult in latestSession.ResultsPositions.OrderBy(p => p.Position))
        {
            var carIdx = positionResult.CarIdx;
            var driver = driverInfo.Drivers[carIdx];
            
            if (!int.TryParse(driver.CarNumber, out var carNumber))
                carNumber = -1;
            
            timesheetEntries.Add(new TimesheetEntryModel
            {
                FullName = driver.UserName,
                CarModel = driver.CarScreenName,
                CarNumber = carNumber, 
                SkillRating = driver.IRating.ToString(),
                SafetyRating = driver.LicString,
                OverallPosition = positionResult.Position,
                ClassPosition = positionResult.ClassPosition,
                LapsDriven = (int)positionResult.LapsDriven,
                LastLapMs = (int)(positionResult.LastTime * 1000),
                IsLocal = carIdx == driverInfo.DriverCarIdx
            });
        }

        return timesheetEntries;
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