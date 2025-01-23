using System.Collections.ObjectModel;
using System.Windows;
using RacingAidWpf.Configuration;
using RacingAidWpf.Extensions;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public class TimesheetOverlayViewModel : NotifyPropertyChanged
{
    private static readonly TimesheetConfigSection TimesheetConfigSection = ConfigSectionSingleton.TimesheetSection;
    
    private ObservableCollection<TimesheetGridRow> timesheet = [];
    public ObservableCollection<TimesheetGridRow> Timesheet
    {
        get => timesheet;
        private set
        {
            if (timesheet == value)
                return;
            
            timesheet = value;
            OnPropertyChanged();
        }
    }
    
    #region Visibility properties

    public Visibility CarNumberColumnVisibility => TimesheetConfigSection.DisplayCarNumber.ToVisibility();
    public Visibility SafetyColumnVisibility => TimesheetConfigSection.DisplaySafetyRating.ToVisibility();
    public Visibility SkillColumnVisibility => TimesheetConfigSection.DisplaySkillRating.ToVisibility();
    public Visibility LastLapColumnVisibility => TimesheetConfigSection.DisplayLastLap.ToVisibility();
    public Visibility FastestLapColumnVisibility => TimesheetConfigSection.DisplayFastestLap.ToVisibility();
    public Visibility GapToLeaderColumnVisibility => TimesheetConfigSection.DisplayGapToLeader.ToVisibility();
    
    #endregion
    
    public TimesheetOverlayViewModel()
    {
        RacingAidUpdateDispatch.Update += UpdateProperties;

        TimesheetConfigSection.ConfigUpdated += OnConfigUpdated;
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

        var entriesToDisplay = Math.Min(newDrivers.Count, 15);
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
        OnPropertyChanged(nameof(CarNumberColumnVisibility));
        OnPropertyChanged(nameof(SafetyColumnVisibility));
        OnPropertyChanged(nameof(SkillColumnVisibility));
        OnPropertyChanged(nameof(LastLapColumnVisibility));
        OnPropertyChanged(nameof(FastestLapColumnVisibility));
        OnPropertyChanged(nameof(GapToLeaderColumnVisibility));
    }
}