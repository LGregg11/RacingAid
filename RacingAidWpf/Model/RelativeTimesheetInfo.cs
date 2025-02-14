namespace RacingAidWpf.Model;

public class RelativeTimesheetInfo(
    int position,
    int positionInClass,
    string name,
    string skillRating,
    string safetyRating,
    string carModel,
    int carNumber,
    int lastLapTimeMs = 0,
    int fastestLapTimeMs = 0,
    int deltaToLocalMs = 0,
    float lapDistancePercentage = 0f,
    bool isLocal = false,
    bool inPits = false) :
    TimesheetInfo(
        position,
        positionInClass,
        name,
        skillRating,
        safetyRating,
        carModel,
        carNumber,
        lastLapTimeMs,
        fastestLapTimeMs,
        isLocal,
        inPits)
{
    public int DeltaToLocalMs { get; set; } = deltaToLocalMs;
    public float LapDistancePercentage { get; set; } = lapDistancePercentage;
}