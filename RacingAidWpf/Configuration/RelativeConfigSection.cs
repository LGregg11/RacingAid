namespace RacingAidWpf.Configuration;

public class RelativeConfigSection(IConfig config) : ConfigSection(config, "Relative")
{
    public int MaxPositionsAheadOrBehind
    {
        get => GetInt(nameof(MaxPositionsAheadOrBehind), 3);
        set => SetValue(nameof(MaxPositionsAheadOrBehind), value.ToString());
    }
    
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
    
    public bool DisplayGapToLocal
    {
        get => GetBool(nameof(DisplayGapToLocal));
        set => SetValue(nameof(DisplayGapToLocal), value.ToString());
    }
}