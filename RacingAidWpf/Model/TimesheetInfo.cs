namespace RacingAidWpf.Model;

public class TimesheetInfo(
    int position,
    int positionInClass,
    string name,
    string skillRating,
    string safetyRating,
    string carModel,
    int carNumber,
    int lastLapTimeMs = 0,
    int fastestLapTimeMs = 0,
    bool isLocal = false,
    bool inPits = false) : IEquatable<TimesheetInfo>
{

    public string CarModel { get; } = carModel;
    public int CarNumber { get; } = carNumber;
    public int PositionOverall { get; set; } = position;
    public int PositionInClass { get; set; } = positionInClass;
    public string FullName { get; set; } = name;
    public string SkillRating { get; set; } = skillRating;
    public string SafetyRating { get; set; } = safetyRating;
    public int LastLapTimeMs { get; set; } = lastLapTimeMs;
    public int FastestLapTimeMs { get; set; } = fastestLapTimeMs;
    public bool IsLocal { get; set; } = isLocal;

    public bool InPits { get; set; } = inPits;

    public static bool operator ==(TimesheetInfo left, TimesheetInfo right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TimesheetInfo left, TimesheetInfo right)
    {
        return !(left == right);
    }

    public bool Equals(TimesheetInfo other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return CarModel == other.CarModel && CarNumber == other.CarNumber;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TimesheetInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CarModel, CarNumber);
    }
}