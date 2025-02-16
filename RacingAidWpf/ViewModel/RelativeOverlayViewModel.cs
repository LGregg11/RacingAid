using System.Collections.ObjectModel;
using System.Windows;
using RacingAidWpf.Core.Configuration;
using RacingAidWpf.Core.Dispatchers;
using RacingAidWpf.Core.Logging;
using RacingAidWpf.Core.Overlays;
using RacingAidWpf.Core.Singleton;
using RacingAidWpf.Core.Timesheets.Relative;
using RacingAidWpf.Extensions;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public class RelativeOverlayViewModel : OverlayViewModel
{
    private static readonly RelativeConfigSection RelativeConfigSection = ConfigSectionSingleton.RelativeSection;
    private readonly RelativeTimesheet relativeTimesheet;
    
    private ObservableCollection<RelativeTimesheetInfo> relative = [];
    public ObservableCollection<RelativeTimesheetInfo> Relative
    {
        get => relative;
        private set
        {
            if (relative == value)
                return;
            
            relative = value;
            InvokeOnMainThread(() => OnPropertyChanged());
        }
    }
    
    #region Visibility properties

    public Visibility CarNumberColumnVisibility => RelativeConfigSection.DisplayCarNumber.ToVisibility();
    public Visibility SafetyColumnVisibility => RelativeConfigSection.DisplaySafetyRating.ToVisibility();
    public Visibility SkillColumnVisibility => RelativeConfigSection.DisplaySkillRating.ToVisibility();
    public Visibility LastLapColumnVisibility => RelativeConfigSection.DisplayLastLap.ToVisibility();
    public Visibility FastestLapColumnVisibility => RelativeConfigSection.DisplayFastestLap.ToVisibility();
    public Visibility DeltaToLocalColumnVisibility => RelativeConfigSection.DisplayGapToLocal.ToVisibility();
    
    #endregion
    
    public RelativeOverlayViewModel(RelativeTimesheet relativeTimesheet = null, ILogger logger = null)
    {
        Logger = logger ?? LoggerFactory.GetLogger<RelativeOverlayViewModel>();
        this.relativeTimesheet = relativeTimesheet ?? new RelativeTimesheet();
        
        RacingAidUpdateDispatch.Update += UpdateRelative;
        RelativeConfigSection.ConfigUpdated += OnConfigUpdated;
    }

    public override void Reset()
    {
        Logger?.LogDebug($"Resetting {nameof(Relative)}");
        relativeTimesheet.Clear();
        Relative =
            new ObservableCollection<RelativeTimesheetInfo>(relativeTimesheet.RelativeEntries);
    }

    private void UpdateRelative()
    {
        if (RacingAidSingleton.Instance.Relative is not { LocalEntry: {} localEntry } relativeModel)
            return;
        
        relativeTimesheet.UpdateFromData(relativeModel);
        var relativeEntries = relativeTimesheet.RelativeEntries.ToList();
        
        // Only display relative entries up to a maximum specified in the configs
        var entriesAheadOrBehind = RelativeConfigSection.MaxPositionsAheadOrBehind;
        var currentDriverIndex = relativeEntries.FindIndex(r => r.CarNumber == localEntry.CarNumber);
        var minEntryIndex = Math.Max(currentDriverIndex - entriesAheadOrBehind, 0);
        var maxEntryIndex = Math.Min(currentDriverIndex + entriesAheadOrBehind + 1, relativeEntries.Count - 1);

        var relativeGridRowsToDisplay = relativeEntries.GetRange(minEntryIndex, maxEntryIndex - minEntryIndex);
        Relative = new ObservableCollection<RelativeTimesheetInfo>(relativeGridRowsToDisplay);
    }

    private void OnConfigUpdated()
    {
        OnPropertyChanged(nameof(CarNumberColumnVisibility));
        OnPropertyChanged(nameof(SafetyColumnVisibility));
        OnPropertyChanged(nameof(SkillColumnVisibility));
        OnPropertyChanged(nameof(LastLapColumnVisibility));
        OnPropertyChanged(nameof(FastestLapColumnVisibility));
        OnPropertyChanged(nameof(DeltaToLocalColumnVisibility));
    }
}