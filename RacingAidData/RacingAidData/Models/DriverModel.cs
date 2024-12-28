namespace RacingAidData.Models;

public class DriverModel
{
    /// <summary>
    /// Driver name
    /// </summary>
    public string FullName { get; private set; }
    
    /// <summary>
    /// The skill rating
    /// </summary>
    public string SkillRating { get; private set; }
    
    /// <summary>
    /// The safety rating
    /// </summary>
    public string SafetyRating { get; private set; }

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="fullName">The driver's full name</param>
    /// <param name="skillRating">The driver's skill rating</param>
    /// <param name="safetyRating">The driver's safety rating</param>
    public DriverModel(string fullName, string skillRating, string safetyRating)
    {
        FullName = fullName;
        SkillRating = skillRating;
        SafetyRating = safetyRating;
    }
}