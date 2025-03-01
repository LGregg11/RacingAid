﻿using IRSDKSharper;
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

        // Leaderboard timesheet data
        if (CreateTimesheetEntries(iRacingData) is { Count: > 0 } timesheetEntries)
        {
            var timesheetModel = new TimesheetModel<LeaderboardEntryModel>
            {
                Entries = timesheetEntries,
                LocalEntry = timesheetEntries.FirstOrDefault(e => e.IsLocal)
            };
    
            models.Add(timesheetModel);
        }

        // Relative timesheet data
        if (CreateRelativeEntries(iRacingData) is { Count: > 0 } relativeEntries)
        {
            var relativeModel = new TimesheetModel<RelativeEntryModel>
            {
                Entries = relativeEntries,
                LocalEntry = relativeEntries.FirstOrDefault(e => e.IsLocal)
            };
                
            models.Add(relativeModel);
        }
        
        // Telemetry data
        if (CreateTelemetryModel(iRacingData) is { } raceDataModel)
            models.Add(raceDataModel);
        
        // Driver data .. data
        if (CreateDriverModel(iRacingData) is { } driverDataModel)
            models.Add(driverDataModel);
        
        if (CreateTrackModel(iRacingData) is {} trackDataModel)
            models.Add(trackDataModel);
        
        return models.Count > 0;
    }

    private static List<LeaderboardEntryModel> CreateTimesheetEntries(IRacingSdkData iRacingData)
    {
        var timesheetEntries = new List<LeaderboardEntryModel>();

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
            
            timesheetEntries.Add(new LeaderboardEntryModel
            {
                FullName = driver.UserName,
                CarModel = driver.CarScreenName,
                CarNumber = carNumber, 
                SkillRating = driver.IRating.ToString(),
                SafetyRating = driver.LicString,
                OverallPosition = resultPosition.Position,
                ClassPosition = resultPosition.ClassPosition,
                LapsDriven = GetLapsDriven(iRacingData, carIdx),
                LastLapMs = (int)(resultPosition.LastTime * 1000),
                FastestLapMs = (int)(resultPosition.FastestTime * 1000),
                GapToLeaderMs = gapToLeaderMs,
                InPits = GetInPits(iRacingData, carIdx),
                IsLocal = carIdx == driverInfo.DriverCarIdx
            });
        }

        return timesheetEntries;
    }

    private static List<RelativeEntryModel> CreateRelativeEntries(IRacingSdkData iRacingData)
    {
        var relativeEntries = new List<RelativeEntryModel>();
        if (iRacingData.SessionInfo?.DriverInfo is not { Drivers: { Count: > 0 } drivers} driverInfo)
            return relativeEntries;

        var localCarIdx = driverInfo.DriverCarIdx;
        var localLapPercentage = GetLapDistancePercentage(iRacingData, localCarIdx);
        var localLapTimeMs = GetEstTimeToPositionOnTrackMs(iRacingData, localCarIdx);
        var resultsPositions = iRacingData.SessionInfo.SessionInfo.Sessions.LastOrDefault()?.ResultsPositions;

        // Loop through position results to ensure the position order is correct - can grab necessary driver info
        foreach (var driver in drivers.OrderBy(d => GetOverallPosition(iRacingData, d.CarIdx)))
        {
            if (!int.TryParse(driver.CarNumber, out var carNumber))
                carNumber = -1;

            var carIdx = driver.CarIdx;

            IRacingSdkSessionInfo.SessionInfoModel.SessionModel.PositionModel resultPosition = new();
            var hasResult = false;
            if (resultsPositions?.FirstOrDefault(t => t.CarIdx == carIdx) is { } position)
            {
                hasResult = true;
                resultPosition = position;
            }

            var lapsDriven = GetLapsDriven(iRacingData, carIdx);
            var lapPercentage = lapsDriven - (int)lapsDriven;
            
            var gapToLocalMs = GetGapToLocalMs(iRacingData, localCarIdx, carIdx);
            if (localLapPercentage >= 0.5f && lapPercentage < 0.5f)
            {
                var localLapPercentageDelta = -1f * (lapPercentage + (1f - localLapPercentage));
                gapToLocalMs = (int)(localLapTimeMs * localLapPercentageDelta / localLapPercentage);
            }

            relativeEntries.Add(new RelativeEntryModel
            {
                FullName = driver.UserName,
                CarModel = driver.CarScreenName,
                CarNumber = carNumber, 
                SkillRating = driver.IRating.ToString(),
                SafetyRating = driver.LicString,
                OverallPosition = GetOverallPosition(iRacingData, carIdx), // might not have a 'result' yet
                ClassPosition = GetClassPosition(iRacingData, carIdx), // might not have a 'result' yet
                LapsDriven = lapsDriven,
                LastLapMs = hasResult ? (int)(resultPosition.LastTime * 1000) : 0,
                FastestLapMs = hasResult ? (int)(resultPosition.FastestTime * 1000) : 0,
                GapToLocalMs = gapToLocalMs,
                InPits = GetInPits(iRacingData, carIdx),
                IsLocal = carIdx == driverInfo.DriverCarIdx
            });
        }

        return relativeEntries;
    }

    private static RaceDataModel CreateTelemetryModel(IRacingSdkData iRacingData)
    {
        return new TelemetryModel
        {
            ThrottleInput = iRacingData.GetFloat("Throttle"),
            BrakeInput = iRacingData.GetFloat("Brake"),
            ClutchInput = iRacingData.GetFloat("Clutch"),
            SpeedMs = iRacingData.GetFloat("Speed"),
            Gear = iRacingData.GetInt("Gear"),
            Rpm = iRacingData.GetFloat("RPM"),
            SteeringAngleDegrees = iRacingData.GetFloat("SteeringWheelAngle") * RadToDeg
        };
    }

    private static DriverDataModel CreateDriverModel(IRacingSdkData iRacingData)
    {
        var lapsDriven = 0f;
        var inPits = false;
        var incidents = 0;
        if (iRacingData.SessionInfo?.DriverInfo is
            { Drivers: { Count: > 0 }, DriverCarIdx: { } driverCarIdx } driverInfo)
        {
            lapsDriven = GetLapsDriven(iRacingData, driverCarIdx);
            inPits = GetInPits(iRacingData, driverCarIdx);
            incidents = driverInfo.DriverIncidentCount;
        }
        
        return new DriverDataModel
        {
            VelocityMs = GetVelocity(iRacingData),
            ForwardDirectionDeg = iRacingData.GetFloat("YawNorth") * RadToDeg,
            PitchDeg = -1f * iRacingData.GetFloat("Pitch") * RadToDeg,
            RollDeg = iRacingData.GetFloat("Roll") * RadToDeg,
            LapsDriven = lapsDriven,
            LapDistanceMetres  = iRacingData.GetFloat("LapDist"),
            InPits = inPits,
            Incidents = incidents
        };
    }

    private static bool GetInPits(IRacingSdkData iRacingData, int driverCarIdx)
    {
        return iRacingData.GetBool("CarIdxOnPitRoad", driverCarIdx);
    }

    private static Velocity GetVelocity(IRacingSdkData iRacingData)
    {
        return new Velocity(
            iRacingData.GetFloat("VelocityX"),
            iRacingData.GetFloat("VelocityY"),
            iRacingData.GetFloat("VelocityZ"));
    }

    private static TrackDataModel CreateTrackModel(IRacingSdkData iRacingData)
    {
        var trackDataModel = new TrackDataModel();
        
        if (iRacingData.SessionInfo?.WeekendInfo?.TrackName is {} trackName)
            trackDataModel.TrackName = trackName;
        
        return trackDataModel;
    }

    private static int GetGapToLeaderMs(IRacingSdkData iRacingData, int carIdx)
    {
        return (int)(iRacingData.GetFloat("CarIdxF2Time", carIdx) * 1000f);
    }

    private static int GetGapToLocalMs(IRacingSdkData iRacingData, int localCarIdx, int carIdx)
    {
        return GetEstTimeToPositionOnTrackMs(iRacingData, localCarIdx) -
               GetEstTimeToPositionOnTrackMs(iRacingData, carIdx);
    }

    private static int GetEstTimeToPositionOnTrackMs(IRacingSdkData iRacingData, int carIdx)
    {
        return (int)(iRacingData.GetFloat("CarIdxEstTime", carIdx) * 1000f);
    }

    private static float GetLap(IRacingSdkData iRacingData, int carIdx)
    {
        return iRacingData.GetInt("CarIdxLap", carIdx);
    }
    
    private static float GetLapDistancePercentage(IRacingSdkData iRacingData, int carIdx)
    {
        return iRacingData.GetFloat("CarIdxLapDistPct", carIdx);
    }
    
    private static float GetLapsDriven(IRacingSdkData iRacingData, int carIdx)
    {
        return GetLap(iRacingData, carIdx) + GetLapDistancePercentage(iRacingData, carIdx);
    }

    private static int GetOverallPosition(IRacingSdkData iRacingData, int carIdx)
    {
        return iRacingData.GetInt("CarIdxPosition", carIdx);
    }

    private static int GetClassPosition(IRacingSdkData iRacingData, int carIdx)
    {
        return iRacingData.GetInt("CarIdxClassPosition", carIdx);
    }
}