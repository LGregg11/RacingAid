using RacingAidData.Core.Models;
using RacingAidWpf.Model;

namespace RacingAidWpf.Core.Timesheets.Relative;

public class RelativeTimesheet : Timesheet<RelativeEntryModel>
{
    public IEnumerable<RelativeTimesheetInfo> RelativeEntries => Entries.OfType<RelativeTimesheetInfo>();

    protected override TimesheetInfo CreateTimesheetInfo(RelativeEntryModel relativeEntryModel)
    {
        return new RelativeTimesheetInfo(
            relativeEntryModel.OverallPosition,
            relativeEntryModel.ClassPosition,
            relativeEntryModel.FullName,
            relativeEntryModel.SkillRating,
            relativeEntryModel.SafetyRating,
            relativeEntryModel.CarModel,
            relativeEntryModel.CarNumber,
            relativeEntryModel.LastLapMs,
            relativeEntryModel.FastestLapMs,
            relativeEntryModel.GapToLocalMs,
            relativeEntryModel.LapsDriven,
            relativeEntryModel.IsLocal,
            relativeEntryModel.InPits);
    }

    protected override List<TimesheetInfo> CreateTimesheetEntries(TimesheetModel<RelativeEntryModel> relativeData)
    {
        var localLapPercentage = relativeData.LocalEntry?.LapPercentage ?? 0f;
        
        List<RelativeTimesheetInfo> relativeInfoEntries = [];
        foreach (var relativeEntry in relativeData.Entries)
        {
            if (CreateTimesheetInfo(relativeEntry) is not RelativeTimesheetInfo relativeTimesheetInfo || relativeTimesheetInfo.LapsDriven < 0)
                continue;
            
            var currentLapPercentageDelta = CalculateBoundedLapPercentageDelta(relativeEntry.LapPercentage, localLapPercentage);
            
            // Simple sort to order info by lap distance relative to local lap position
            var index = relativeInfoEntries.Count(e =>
                CalculateBoundedLapPercentageDelta(e.LapPercentage, localLapPercentage) > currentLapPercentageDelta);
            relativeInfoEntries.Insert(index, relativeTimesheetInfo);
        }

        return new List<TimesheetInfo>(relativeInfoEntries);
    }

    private static float CalculateBoundedLapPercentageDelta(float lapPercentage, float localLapPercentage)
    {
        const float fullLapPercentage = 1f;
        const float halfLapPercentage = 0.5f;
        const float startLapPercentage = 0f;
        
        // local driver info will be at 0.5. All info should be between 0 and 1, so bring back into range
        var updatedLapPercentageDelta = lapPercentage - (localLapPercentage - halfLapPercentage);
        switch (updatedLapPercentageDelta)
        {
            case > fullLapPercentage:
                updatedLapPercentageDelta -= fullLapPercentage;
                break;
            case < startLapPercentage:
                updatedLapPercentageDelta += fullLapPercentage;
                break;
        }
        
        return updatedLapPercentageDelta;
    }
}