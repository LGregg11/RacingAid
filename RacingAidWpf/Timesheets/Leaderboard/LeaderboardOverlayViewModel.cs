using System.Collections.ObjectModel;
using System.Windows;
using RacingAidWpf.Configuration;
using RacingAidWpf.Dispatchers;
using RacingAidWpf.Extensions;
using RacingAidWpf.Logging;
using RacingAidWpf.Model;
using RacingAidWpf.Overlays;
using RacingAidWpf.Singleton;

namespace RacingAidWpf.Timesheets.Leaderboard;

public class LeaderboardOverlayViewModel : OverlayViewModel
{
    private static readonly LeaderboardConfigSection LeaderboardConfigSection = ConfigSectionSingleton.LeaderboardSection;
    
    private ObservableCollection<TimesheetGridRow> timesheet = [];
    public ObservableCollection<TimesheetGridRow> Timesheet
    {
        get => timesheet;
        private set
        {
            if (timesheet == value)
                return;
            
            timesheet = value;
            InvokeOnMainThread(() =>
            {
                OnPropertyChanged();
            });
        }
    }
    
    #region Visibility properties

    public Visibility CarNumberColumnVisibility => LeaderboardConfigSection.DisplayCarNumber.ToVisibility();
    public Visibility SafetyColumnVisibility => LeaderboardConfigSection.DisplaySafetyRating.ToVisibility();
    public Visibility SkillColumnVisibility => LeaderboardConfigSection.DisplaySkillRating.ToVisibility();
    public Visibility LastLapColumnVisibility => LeaderboardConfigSection.DisplayLastLap.ToVisibility();
    public Visibility FastestLapColumnVisibility => LeaderboardConfigSection.DisplayFastestLap.ToVisibility();
    public Visibility GapToLeaderColumnVisibility => LeaderboardConfigSection.DisplayGapToLeader.ToVisibility();
    
    #endregion
    
    public LeaderboardOverlayViewModel(ILogger logger = null)
    {
        Logger = logger ?? LoggerFactory.GetLogger<LeaderboardOverlayViewModel>();
        
        RacingAidUpdateDispatch.Update += UpdateProperties;

        LeaderboardConfigSection.ConfigUpdated += OnConfigUpdated;
    }

    public override void Reset()
    {
        Logger?.LogDebug($"Resetting {nameof(timesheet)}");
        timesheet = [];
    }

    private void UpdateProperties()
    {
        UpdateDriversDataGrid();
    }

    private void UpdateDriversDataGrid()
    {
        var newDrivers = RacingAidSingleton.Instance.Timesheet.Entries;

        if (newDrivers.Count == 0)
        {
            Timesheet = [];
            return;
        }
        
        ObservableCollection<TimesheetGridRow> newTimesheet = [];

        var entriesToDisplay = Math.Min(newDrivers.Count, LeaderboardConfigSection.MaxPositions);
        for (var i=0; i < entriesToDisplay; i++)
        {
            var driver = newDrivers[i];

            newTimesheet.Add(
                new TimesheetGridRow(
                    i+1,
                    0,
                    driver.FullName,
                    driver.SkillRating,
                    driver.SafetyRating,
                    driver.CarModel,
                    driver.CarNumber,
                    driver.LastLapMs,
                    driver.FastestLapMs,
                    driver.GapToLeaderMs,
                    driver.IsLocal));
        }

        Timesheet = newTimesheet;
    }

    private void OnConfigUpdated()
    {
        InvokeOnMainThread(() =>
        {
            OnPropertyChanged(nameof(CarNumberColumnVisibility));
            OnPropertyChanged(nameof(SafetyColumnVisibility));
            OnPropertyChanged(nameof(SkillColumnVisibility));
            OnPropertyChanged(nameof(LastLapColumnVisibility));
            OnPropertyChanged(nameof(FastestLapColumnVisibility));
            OnPropertyChanged(nameof(GapToLeaderColumnVisibility));
        });
    }
}