namespace RacingAidData.Core.Models;

public class TimesheetEntryModel : TimingEntryModel
{
    /// <summary>
    /// Gap to leader in milliseconds
    /// </summary>
    public int GapToLeaderMs { get; init; }
}