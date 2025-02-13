﻿namespace RacingAidData.Core.Models;

/// <summary>
/// Entry for a timesheet (i.e. 'Results' or 'Relative' times)
/// </summary>
public abstract class TimingEntryModel
{
    /// <summary>
    /// Overall position
    /// </summary>
    public int OverallPosition { get; init; }
    
    /// <summary>
    /// Class position
    /// </summary>
    public int ClassPosition { get; init; }
    
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
    
    /// <summary>
    /// Last lap time in milliseconds
    /// </summary>
    public int LastLapMs { get; init; }
    
    /// <summary>
    /// Fastest lap time in milliseconds
    /// </summary>
    public int FastestLapMs { get; init; }
    
    /// <summary>
    /// Laps driven
    /// </summary>
    public float LapsDriven { get; init; }
    
    /// <summary>
    /// Whether this entry is of the local user
    /// </summary>
    public bool IsLocal { get; init; }
}