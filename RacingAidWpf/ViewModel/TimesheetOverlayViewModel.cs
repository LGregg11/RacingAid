using System.Collections.ObjectModel;
using System.Windows;
using RacingAidWpf.Configuration;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public class TimesheetOverlayViewModel : NotifyPropertyChanged
{
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

    public Visibility CarNumberColumnVisibility => ConfigSectionSingleton.TimesheetSection.DisplayCarNumber ? Visibility.Visible : Visibility.Hidden;
    public Visibility SafetyColumnVisibility => ConfigSectionSingleton.TimesheetSection.DisplaySafetyRating ? Visibility.Visible : Visibility.Hidden;
    public Visibility SkillColumnVisibility => ConfigSectionSingleton.TimesheetSection.DisplaySkillRating ? Visibility.Visible : Visibility.Hidden;
    public Visibility LastLapColumnVisibility => ConfigSectionSingleton.TimesheetSection.DisplayLastLap ? Visibility.Visible : Visibility.Hidden;
    public Visibility FastestLapColumnVisibility => ConfigSectionSingleton.TimesheetSection.DisplayFastestLap ? Visibility.Visible : Visibility.Hidden;
    public Visibility GapToLeaderColumnVisibility => ConfigSectionSingleton.TimesheetSection.DisplayGapToLeader ? Visibility.Visible : Visibility.Hidden;
    
    #endregion
    
    public TimesheetOverlayViewModel()
    {
        RacingAidUpdateDispatch.Update += UpdateProperties;

        ConfigSectionSingleton.TimesheetSection.ConfigUpdated += OnConfigUpdated;
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