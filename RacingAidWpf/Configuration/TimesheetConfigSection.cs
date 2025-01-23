﻿namespace RacingAidWpf.Configuration;

public class TimesheetConfigSection(IConfig config) : ConfigSection(config, "Timesheet")
{
    public bool DisplayCarNumber
    {
        get => GetBool(nameof(DisplayCarNumber));
        set => SetValue(nameof(DisplayCarNumber), value.ToString());
    }
    
    public bool DisplaySafetyRating
    {
        get => GetBool(nameof(DisplaySafetyRating));
        set => SetValue(nameof(DisplaySafetyRating), value.ToString());
    }
    
    public bool DisplaySkillRating
    {
        get => GetBool(nameof(DisplaySkillRating));
        set => SetValue(nameof(DisplaySkillRating), value.ToString());
    }
    
    public bool DisplayLastLap
    {
        get => GetBool(nameof(DisplayLastLap));
        set => SetValue(nameof(DisplayLastLap), value.ToString());
    }
    
    public bool DisplayFastestLap
    {
        get => GetBool(nameof(DisplayFastestLap));
        set => SetValue(nameof(DisplayFastestLap), value.ToString());
    }
    
    public bool DisplayGapToLeader
    {
        get => GetBool(nameof(DisplayGapToLeader));
        set => SetValue(nameof(DisplayGapToLeader), value.ToString());
    }
}