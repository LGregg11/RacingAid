namespace RacingAid.Core.Models;

public sealed class DriversModel : RaceDataModel
{
    public override DataType DataType => DataType.Drivers;
    
    public HashSet<DriverModel> Drivers { get; init; }
}