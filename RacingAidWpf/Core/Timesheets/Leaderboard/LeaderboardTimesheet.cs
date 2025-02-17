using RacingAidData.Core.Models;
using RacingAidWpf.Model;

namespace RacingAidWpf.Core.Timesheets.Leaderboard;

public class LeaderboardTimesheet : Timesheet<LeaderboardEntryModel>
{
    public IEnumerable<LeaderboardTimesheetInfo> LeaderboardEntries => Entries.OfType<LeaderboardTimesheetInfo>();
    
    protected override TimesheetInfo CreateTimesheetInfo(LeaderboardEntryModel timesheetEntryData)
    {
        return new LeaderboardTimesheetInfo(
            timesheetEntryData.OverallPosition,
            timesheetEntryData.ClassPosition,
            timesheetEntryData.FullName,
            timesheetEntryData.SkillRating,
            timesheetEntryData.SafetyRating,
            timesheetEntryData.CarModel,
            timesheetEntryData.CarNumber,
            timesheetEntryData.LastLapMs,
            timesheetEntryData.FastestLapMs,
            timesheetEntryData.GapToLeaderMs,
            timesheetEntryData.IsLocal,
            timesheetEntryData.InPits);
    }
}