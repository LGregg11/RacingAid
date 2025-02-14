using RacingAidData.Core.Models;
using RacingAidWpf.Model;

namespace RacingAidWpf.Core.Timesheets.Relative;

public class RelativeTimesheetController : TimesheetController<RelativeEntryModel>
{
    protected override TimesheetInfo CreateTimesheetInfo(RelativeEntryModel timesheetEntryData)
    {
        return new RelativeTimesheetInfo(
            timesheetEntryData.OverallPosition,
            timesheetEntryData.ClassPosition,
            timesheetEntryData.FullName,
            timesheetEntryData.SkillRating,
            timesheetEntryData.SafetyRating,
            timesheetEntryData.CarModel,
            timesheetEntryData.CarNumber,
            timesheetEntryData.LastLapMs,
            timesheetEntryData.FastestLapMs,
            timesheetEntryData.GapToLocalMs,
            timesheetEntryData.LapDistancePercentage,
            timesheetEntryData.IsLocal);
    }
}