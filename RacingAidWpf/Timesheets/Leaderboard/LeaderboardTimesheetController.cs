using RacingAidData.Core.Models;
using RacingAidWpf.Model;

namespace RacingAidWpf.Timesheets.Leaderboard;

public class LeaderboardTimesheetController : TimesheetController<LeaderboardEntryModel>
{
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
            timesheetEntryData.IsLocal);
    }
}