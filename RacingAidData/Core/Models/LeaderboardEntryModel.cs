namespace RacingAidData.Core.Models;

public class LeaderboardEntryModel : TimesheetEntryModel
{
    /// <summary>
    /// Gap to leader in milliseconds
    /// </summary>
    public int GapToLeaderMs { get; init; }
}
