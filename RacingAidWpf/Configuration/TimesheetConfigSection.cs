namespace RacingAidWpf.Configuration;

public class TimesheetConfigSection : ConfigSection
{
    public bool CarNumberColumnVisibility { get; private init; } = true;
    public bool SafetyColumnVisibility { get; private init; } = true;
    public bool SkillColumnVisibility { get; private init; } = true;
    public bool LastLapColumnVisibility { get; private init; } = true;
    public bool FastestLapColumnVisibility { get; private init; } = true;
    public bool LeaderColumnVisibility { get; private init; } = true;
    
    public TimesheetConfigSection(IConfig config) : base(config, "Timesheet")
    {
        CarNumberColumnVisibility = GetValue(nameof(CarNumberColumnVisibility), CarNumberColumnVisibility);
        SafetyColumnVisibility = GetValue(nameof(SafetyColumnVisibility), SafetyColumnVisibility);
        SkillColumnVisibility = GetValue(nameof(SkillColumnVisibility), SkillColumnVisibility);
        LastLapColumnVisibility = GetValue(nameof(LastLapColumnVisibility), LastLapColumnVisibility);
        FastestLapColumnVisibility = GetValue(nameof(FastestLapColumnVisibility), FastestLapColumnVisibility);
        LeaderColumnVisibility = GetValue(nameof(LeaderColumnVisibility), LeaderColumnVisibility);
    }
}