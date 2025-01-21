using System.Windows.Forms;
using RacingAidWpf.Configuration;

namespace RacingAidWpf.WindowManagement;

/// <summary>
/// Use this class to determine which monitor to use and where to place windows
/// </summary>
public static class ScreenDetector
{
    private static readonly GeneralConfigSection GeneralConfigSection = ConfigSectionSingleton.GeneralSection;

    public static IEnumerable<int> ValidScreens => Enumerable.Range(1, Screen.AllScreens.Length);

    /// <summary>
    /// 1, 2, 3 (not 0, 1, 2)
    /// </summary>
    public static int PrimaryScreen
    {
        get => GeneralConfigSection.PrimaryScreen;
        set => GeneralConfigSection.PrimaryScreen = value;
    }
}