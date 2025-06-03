namespace RacingAidData.Core.Models;

public abstract class RaceDataModel
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
}