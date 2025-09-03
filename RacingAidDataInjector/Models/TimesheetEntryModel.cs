namespace RacingAidDataInjector.Models;

/// <summary>
/// Entry for a timesheet (i.e. 'Results' or 'Relative' times)
/// </summary>
public abstract class TimesheetEntryModel
{
    /// <summary>
    /// Overall position
    /// </summary>
    public int OverallPosition { get; set; }
    
    /// <summary>
    /// Class position
    /// </summary>
    public int ClassPosition { get; set; }
    
    /// <summary>
    /// Driver name
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// The skill rating
    /// </summary>
    public string SkillRating { get; set; }
    
    /// <summary>
    /// The safety rating
    /// </summary>
    public string SafetyRating { get; set; }
    
    /// <summary>
    /// The model of the car the driver is racing
    /// </summary>
    public string CarModel { get; set; }
    
    /// <summary>
    /// The number the car is racing under
    /// </summary>
    public int CarNumber { get; set; }
    
    /// <summary>
    /// Last lap time in milliseconds
    /// </summary>
    public int LastLapMs { get; set; }
    
    /// <summary>
    /// Fastest lap time in milliseconds
    /// </summary>
    public int FastestLapMs { get; set; }
    
    /// <summary>
    /// Laps driven
    /// </summary>
    public float LapsDriven { get; set; }

    /// <summary>
    /// Lap percentage (0->1)
    /// </summary>
    public float LapPercentage => LapsDriven - (int)LapsDriven;
    
    /// <summary>
    /// Whether they are in the pits
    /// </summary>
    public bool InPits { get; set; }
    
    /// <summary>
    /// Whether this entry is of the local user
    /// </summary>
    public bool IsLocal { get; set; }
}