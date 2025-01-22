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
        if (CreateTimesheetEntries(iRacingData) is { Count: > 0 } entries)
        {
            var timesheetModel = new TimesheetModel
            {
                Entries = entries,
                LocalEntry = entries.FirstOrDefault(e => e.IsLocal)
            };
    
            models.Add(timesheetModel);
        }
        
        // Telemetry data
        models.Add(CreateTelemetryModel(iRacingData));

        return true;
    }

    private static List<TimesheetEntryModel> CreateTimesheetEntries(IRacingSdkData iRacingData)
    {
        var timesheetEntries = new List<TimesheetEntryModel>();

        if (iRacingData.SessionInfo?.DriverInfo is not { } driverInfo ||
            iRacingData.SessionInfo.SessionInfo.Sessions.LastOrDefault() is not
                { ResultsPositions: { Count: > 0 } resultsPositions })
        {
            return timesheetEntries;
        }

        var driverCount = driverInfo.Drivers.Count;

        var orderedResultPositions = resultsPositions.OrderBy(p => p.Position);
        var leaderTimeMs = -1; 
        
        // Loop through position results to ensure the position order is correct - can grab necessary driver info
        foreach (var resultPosition in orderedResultPositions)
        {
            var carIdx = resultPosition.CarIdx;
            if (carIdx >= driverCount)
                continue;

            if (driverInfo.Drivers.FirstOrDefault(d => d.CarIdx == carIdx) is not { } driver)
                continue;
                
            if (leaderTimeMs < 0)
                leaderTimeMs = GetGapToLeaderMs(iRacingData, driver.CarIdx);
            
            if (!int.TryParse(driver.CarNumber, out var carNumber))
                carNumber = -1;

            var gapToLeaderMs = resultPosition.FastestLap > 0
                ? GetGapToLeaderMs(iRacingData, driver.CarIdx) - leaderTimeMs
                : 0;
            
            timesheetEntries.Add(new TimesheetEntryModel
            {
                FullName = driver.UserName,
                CarModel = driver.CarScreenName,
                CarNumber = carNumber, 
                SkillRating = driver.IRating.ToString(),
                SafetyRating = driver.LicString,
                OverallPosition = resultPosition.Position,
                ClassPosition = resultPosition.ClassPosition,
                LapsDriven = (int)resultPosition.LapsDriven,
                LastLapMs = (int)(resultPosition.LastTime * 1000),
                FastestLapMs = (int)(resultPosition.FastestTime * 1000),
                GapToLeaderMs = gapToLeaderMs,
                IsLocal = carIdx == driverInfo.DriverCarIdx
            });
        }

        return timesheetEntries;
    }

    private static RaceDataModel CreateTelemetryModel(IRacingSdkData iRacingData)
    {
        return new TelemetryModel
        {
            ThrottleInput = iRacingData.GetFloat("Throttle"),
            BrakeInput = iRacingData.GetFloat("Brake"),
            ClutchInput = iRacingData.GetFloat("Clutch"),
            SpeedMetresPerSecond = iRacingData.GetFloat("Speed"),
            Gear = iRacingData.GetInt("Gear"),
            Rpm = iRacingData.GetFloat("RPM"),
            SteeringAngleDegrees = iRacingData.GetFloat("SteeringWheelAngle") * RadToDeg
        };
    }

    private static int GetGapToLeaderMs(IRacingSdkData iRacingData, int carIdx)
    {
        return (int)(iRacingData.GetFloat("CarIdxF2Time", carIdx) * 1000f);
    }
}