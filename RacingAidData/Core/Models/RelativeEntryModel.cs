namespace RacingAidData.Core.Models;

public class RelativeEntryModel : TimingEntryModel
{
    /// <summary>
    /// Gap to local driver (on track) in milliseconds
    /// </summary>
    public int GapToLocalMs { get; init; }
    
    /// <summary>
    /// 0-1 -> 0-100%
    /// </summary>
    public float LapDistancePercentage { get; init; }
}