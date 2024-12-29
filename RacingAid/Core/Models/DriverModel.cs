namespace RacingAid.Core.Models;

public struct DriverModel
{
    /// <summary>
    /// Driver name
    /// </summary>
    public string FullName { get; init; }
    
    /// <summary>
    /// The skill rating
    /// </summary>
    public string SkillRating { get; init; }
    
    /// <summary>
    /// The safety rating
    /// </summary>
    public string SafetyRating { get; init; }
    
    /// <summary>
    /// The model of the car the driver is racing
    /// </summary>
    public string CarModel { get; init; }
    
    /// <summary>
    /// The number the car is racing under
    /// </summary>
    public int CarNumber { get; init; }
}