namespace RacingAidDataInjector.Models;

public class RelativeEntryModel : TimesheetEntryModel
{
    /// <summary>
    /// Gap to local driver (on track) in milliseconds
    /// </summary>
    public int GapToLocalMs { get; init; }
}