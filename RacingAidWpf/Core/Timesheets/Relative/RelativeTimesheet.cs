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
            relativeEntryModel.IsLocal);
    }

    protected override List<TimesheetInfo> CreateTimesheetEntries(TimesheetModel<RelativeEntryModel> relativeData)
    {
        const float fullLapPercentage = 1f;
        const float halfLapPercentage = 0.5f;
        
        var localLapPercentage = relativeData.LocalEntry?.LapPercentage ?? 0f;
        
        List<RelativeTimesheetInfo> relativeInfoEntries = [];
        foreach (var relativeEntry in relativeData.Entries)
        {
            if (CreateTimesheetInfo(relativeEntry) is not RelativeTimesheetInfo relativeTimesheetInfo)
                continue;
            
            // local driver info will be at 0. All info should be between -0.5f and +0.5f, so bring back into range
            var currentInfoLapDelta = relativeTimesheetInfo.LapPercentage - localLapPercentage;
            if (currentInfoLapDelta > halfLapPercentage)
                currentInfoLapDelta -= fullLapPercentage;
            else if (currentInfoLapDelta < halfLapPercentage)
                currentInfoLapDelta += fullLapPercentage;
            
            // Simple sort to order info by lap distance relative to local lap position
            var index = relativeInfoEntries.Count(e => e.LapPercentage - localLapPercentage < currentInfoLapDelta);
            relativeInfoEntries.Insert(index, relativeTimesheetInfo);
        }

        return new List<TimesheetInfo>(relativeInfoEntries);
    }
}