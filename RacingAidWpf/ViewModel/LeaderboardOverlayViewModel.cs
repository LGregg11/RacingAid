using System.Collections.ObjectModel;
using System.Windows;
using RacingAidWpf.Core.Configuration;
using RacingAidWpf.Core.Dispatchers;
using RacingAidWpf.Core.Logging;
using RacingAidWpf.Core.Overlays;
using RacingAidWpf.Core.Singleton;
using RacingAidWpf.Core.Timesheets.Leaderboard;
using RacingAidWpf.Extensions;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public class LeaderboardOverlayViewModel : OverlayViewModel
{
    private static readonly LeaderboardConfigSection LeaderboardConfigSection = ConfigSectionSingleton.LeaderboardSection;
    private readonly LeaderboardTimesheet leaderboardTimesheet;
    
    private ObservableCollection<LeaderboardTimesheetInfo> leaderboard = [];
    public ObservableCollection<LeaderboardTimesheetInfo> Leaderboard
    {
        get => leaderboard;
        private set
        {
            if (leaderboard == value)
                return;
            
            leaderboard = value;
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
    
    public LeaderboardOverlayViewModel(LeaderboardTimesheet leaderboardTimesheet = null, ILogger logger = null)
    {
        Logger = logger ?? LoggerFactory.GetLogger<LeaderboardOverlayViewModel>();
        this.leaderboardTimesheet = leaderboardTimesheet ?? new LeaderboardTimesheet();
        
        RacingAidUpdateDispatch.Update += UpdateLeaderboard;
        LeaderboardConfigSection.ConfigUpdated += OnConfigUpdated;
    }

    public override void Reset()
    {
        Logger?.LogDebug($"Resetting {nameof(Leaderboard)}");
        leaderboardTimesheet.Clear();
        Leaderboard =
            new ObservableCollection<LeaderboardTimesheetInfo>(leaderboardTimesheet.LeaderboardEntries);
    }

    private void UpdateLeaderboard()
    {
        leaderboardTimesheet.UpdateFromData(RacingAidSingleton.Instance.Leaderboard);
        var leaderboardEntries = leaderboardTimesheet.LeaderboardEntries.ToList();
        
        // Only display leaderboard entries up to a maximum specified in the configs
        var entriesToDisplay = Math.Min(leaderboardEntries.Count, LeaderboardConfigSection.MaxPositions);
        leaderboardEntries = leaderboardEntries[..entriesToDisplay]; // new syntax for .Slice(0, n)
        Leaderboard = new ObservableCollection<LeaderboardTimesheetInfo>(leaderboardEntries);
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