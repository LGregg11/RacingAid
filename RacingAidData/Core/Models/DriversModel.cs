namespace RacingAidData.Core.Models;

public sealed class DriversModel : RaceDataModel
{
    public DriverModel LocalDriver { get; init; }
    
    public List<DriverModel> Drivers { get; init; }
}