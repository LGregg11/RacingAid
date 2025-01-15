namespace RacingAidData.Core.Models;

public abstract class RaceDataModel
{
    public DateTime Timestamp { get; } = DateTime.Now;
}