using System.Windows.Forms;
using RacingAidWpf.Configuration;

namespace RacingAidWpf.ScreenManagement;

/// <summary>
/// Use this class to determine which monitor to use and where to place windows
/// </summary>
public class ScreenDetector
{
    private static readonly GeneralConfigSection GeneralConfigSection = ConfigSectionSingleton.GeneralSection;

    /// <summary>
    /// 1, 2, 3 (not 0, 1, 2)
    /// </summary>
    public int PrimaryScreen
    {
        get => GeneralConfigSection.PrimaryScreen;
        set => GeneralConfigSection.PrimaryScreen = value;
    }
    
    public IEnumerable<int> ValidScreens => Enumerable.Range(1, Screen.AllScreens.Length);

    public ScreenDetector()
    {
        GeneralConfigSection.ConfigUpdated += OnConfigUpdated;
    }

    private void OnConfigUpdated()
    {
        
    }
}