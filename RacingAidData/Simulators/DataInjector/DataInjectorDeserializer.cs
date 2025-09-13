using IRSDKSharper;
using RacingAidData.Core.Deserializers;
using RacingAidData.Core.Models;
using RacingAidGrpc;

namespace RacingAidData.Simulators.DataInjector;

#if DEBUG
public class DataInjectorDeserializer : IDeserializeData
{
    public bool TryDeserializeData(object data, out List<RaceDataModel> models)
    {
        models = [];

        if (data is RelativeResponse relativeResponse)
        {
            var relativeEntries = CreateRelativeEntries(relativeResponse);
            var relativeModel = new TimesheetModel<RelativeEntryModel>
            {
                Entries = relativeEntries,
                LocalEntry = relativeEntries.FirstOrDefault(e => e.IsLocal)
            };
            models.Add(relativeModel);
        }
        
        return models.Count > 0;
    }

    private static List<LeaderboardEntryModel> CreateTimesheetEntries(IRacingSdkData iRacingData)
    {
        return [];
    }

    private static List<RelativeEntryModel> CreateRelativeEntries(RelativeResponse relative)
    {
        var relativeEntries = new List<RelativeEntryModel>();
        foreach (var relativeDriver in relative.Drivers)
        {
            var safetyRating = new RacingAidData.Core.Models.SafetyRating
            {
                Rating = relativeDriver.TimesheetEntry.SafetyRating.Rating,
                Color = System.Drawing.Color.FromArgb(relativeDriver.TimesheetEntry.SafetyRating.Color.ARGB)
            };
            
            relativeEntries.Add(new RelativeEntryModel
            {
                OverallPosition = relativeDriver.TimesheetEntry.OverallPosition,
                ClassPosition = relativeDriver.TimesheetEntry.ClassPosition,
                FullName = relativeDriver.TimesheetEntry.FullName,
                CarModel = relativeDriver.TimesheetEntry.CarModel,
                CarNumber = relativeDriver.TimesheetEntry.CarNumber,
                InPits = relativeDriver.TimesheetEntry.InPits,
                IsLocal = relativeDriver.TimesheetEntry.IsLocal,
                LapsDriven = relativeDriver.TimesheetEntry.LapsDriven,
                LastLapMs = relativeDriver.TimesheetEntry.LastLapMs,
                FastestLapMs = relativeDriver.TimesheetEntry.FastestLapMs,
                SkillRating = relativeDriver.TimesheetEntry.SkillRating,
                SafetyRating = safetyRating,
                GapToLocalMs = relativeDriver.GapToLocalMs
            });
        }
        return relativeEntries;
    }

    private static RaceDataModel CreateTelemetryModel(IRacingSdkData iRacingData)
    {
        return new TelemetryModel();
    }

    private static DriverDataModel CreateDriverModel(IRacingSdkData iRacingData)
    {
        return new DriverDataModel();
    }
}
#endif