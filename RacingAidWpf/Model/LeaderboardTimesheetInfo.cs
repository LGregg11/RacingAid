namespace RacingAidWpf.Model;

public class LeaderboardTimesheetInfo(
    int position,
    int positionInClass,
    string name,
    string skillRating,
    string safetyRating,
    string carModel,
    int carNumber,
    int lastLapTimeMs = 0,
    int fastestLapTimeMs = 0,
    int deltaToLeaderMs = 0,
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
    public int DeltaToLeaderMs { get; set; } = deltaToLeaderMs;
}