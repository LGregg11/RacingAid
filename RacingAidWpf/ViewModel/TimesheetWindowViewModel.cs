using System.Collections.ObjectModel;
using RacingAidWpf.Configuration;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public class TimesheetWindowViewModel : NotifyPropertyChanged
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

    public static bool IsCarNumberColumnVisible => ConfigSectionSingleton.TimesheetSection.DisplayCarNumber;
    public static bool IsSafetyColumnVisible => ConfigSectionSingleton.TimesheetSection.DisplaySafetyRating;
    public static bool IsSkillColumnVisible => ConfigSectionSingleton.TimesheetSection.DisplaySkillRating;
    public static bool IsLastLapColumnVisible => ConfigSectionSingleton.TimesheetSection.DisplayLastLap;
    public static bool IsFastestLapColumnVisible => ConfigSectionSingleton.TimesheetSection.DisplayFastestLap;
    public static bool IsGapToLeaderColumnVisible => ConfigSectionSingleton.TimesheetSection.DisplayGapToLeader;
    
    #endregion
    
    public TimesheetWindowViewModel()
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
                    driver.FastestLapMs));
        }

        Timesheet = newTimesheet;
    }

    private void OnConfigUpdated()
    {
        OnPropertyChanged(nameof(IsCarNumberColumnVisible));
        OnPropertyChanged(nameof(IsSafetyColumnVisible));
        OnPropertyChanged(nameof(IsSkillColumnVisible));
        OnPropertyChanged(nameof(IsLastLapColumnVisible));
        OnPropertyChanged(nameof(IsFastestLapColumnVisible));
        OnPropertyChanged(nameof(IsGapToLeaderColumnVisible));
    }
}