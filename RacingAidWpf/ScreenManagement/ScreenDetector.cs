using RacingAidWpf.Configuration;

namespace RacingAidWpf.ScreenManagement;

/// <summary>
/// Use this class to determine which monitor to use and where to place windows
/// </summary>
public class ScreenDetector
{
    private static readonly GeneralConfigSection GeneralConfigSection = ConfigSectionSingleton.GeneralSection;
    
    private int primaryScreenIndex;

    public ScreenDetector()
    {
        primaryScreenIndex = GeneralConfigSection.PrimaryMonitor - 1;

        GeneralConfigSection.ConfigUpdated += OnConfigUpdated;
    }

    private void OnConfigUpdated()
    {
        primaryScreenIndex = GeneralConfigSection.PrimaryMonitor - 1;
    }
}