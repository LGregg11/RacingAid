﻿namespace RacingAidData.Core.Models;

public class DriverDataModel : RaceDataModel
{
    /// <summary>
    /// The velocity of the driver in metres per second
    /// </summary>
    public Velocity? VelocityMs { get; init; }
    
    /// <summary>
    /// The forward direction of the driver's car in degrees (North = 0, West = 270)
    /// </summary>
    public float ForwardDirectionDeg { get; init; }
    
    /// <summary>
    /// The pitch angle of the car in degrees (No pitch = 0, Upwards = 90)
    /// </summary>
    public float PitchDeg { get; init; }
    
    /// <summary>
    /// The roll angle of the car in degrees (No roll = 0, Right = 90)
    /// </summary>
    public float RollDeg { get; init; }
    /// <summary>
    /// The number of laps driven (float to keep track of current lap progress)
    /// </summary>
    public float LapsDriven { get; init; }
    
    /// <summary>
    /// The lap distance covered in metres
    /// </summary>
    public float LapDistanceMetres { get; init; }
    
    /// <summary>
    /// Whether the driver is in the pits
    /// </summary>
    public bool InPits { get; init; }
    
    /// <summary>
    /// The number of incidents/penalties that the driver has accumulated
    /// </summary>
    public int Incidents { get; init; }
}

public class Velocity(float x, float y, float z)
{
    /// <summary>
    /// +ve X = forward
    /// </summary>
    public float X { get; set; } = x;
    
    /// <summary>
    /// +ve Y = left
    /// </summary>
    public float Y { get; set; } = y;
    
    /// <summary>
    /// +ve Z = up
    /// </summary>
    public float Z { get; set; } = z;
}