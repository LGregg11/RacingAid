namespace RacingAid.Core.Models;

public abstract class RaceDataModel
{
    public DateTime Timestamp { get; } = DateTime.Now;
    
    public abstract DataType DataType { get; }
}