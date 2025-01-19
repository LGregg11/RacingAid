using System.Collections.ObjectModel;
using RacingAidWpf.Configuration;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public class TimesheetWindowViewModel : NotifyPropertyChanged
{
    private readonly TimesheetConfigSection timesheetConfigSection;
    
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

    private bool carNumberColumnVisibility = true;
    public bool CarNumberColumnVisibility
    {
        get => carNumberColumnVisibility;
        private set
        {
            if (carNumberColumnVisibility == value)
                return;
            
            carNumberColumnVisibility = value;
            OnPropertyChanged();
        }
    }

    private bool safetyColumnVisibility = true;
    public bool SafetyColumnVisibility
    {
        get => safetyColumnVisibility;
        private set
        {
            if (safetyColumnVisibility == value)
                return;
            
            safetyColumnVisibility = value;
            OnPropertyChanged();
        }
    }

    private bool skillColumnVisibility = true;
    public bool SkillColumnVisibility
    {
        get => skillColumnVisibility;
        private set
        {
            if (skillColumnVisibility == value)
                return;
            
            skillColumnVisibility = value;
            OnPropertyChanged();
        }
    }

    private bool lastLapColumnVisibility = true;
    public bool LastLapColumnVisibility
    {
        get => lastLapColumnVisibility;
        private set
        {
            if (lastLapColumnVisibility == value)
                return;
            
            lastLapColumnVisibility = value;
            OnPropertyChanged();
        }
    }

    private bool fastestLapColumnVisibility = true;
    public bool FastestLapColumnVisibility
    {
        get => fastestLapColumnVisibility;
        private set
        {
            if (fastestLapColumnVisibility == value)
                return;
            
            fastestLapColumnVisibility = value;
            OnPropertyChanged();
        }
    }

    private bool leaderColumnVisibility = true;
    public bool LeaderColumnVisibility
    {
        get => leaderColumnVisibility;
        private set
        {
            if (leaderColumnVisibility == value)
                return;
            
            leaderColumnVisibility = value;
            OnPropertyChanged();
        }
    }
    
    #endregion
    
    public TimesheetWindowViewModel()
    {
        RacingAidUpdateDispatch.Update += UpdateProperties;

        timesheetConfigSection = new TimesheetConfigSection();
        timesheetConfigSection.ConfigUpdated += OnConfigUpdated;
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
        CarNumberColumnVisibility = timesheetConfigSection.DisplayCarNumber;
        SafetyColumnVisibility = timesheetConfigSection.DisplaySafetyRating;
        SkillColumnVisibility = timesheetConfigSection.DisplaySkillRating;
        LastLapColumnVisibility = timesheetConfigSection.DisplayLastLap;
        FastestLapColumnVisibility = timesheetConfigSection.DisplayFastestLap;
        LeaderColumnVisibility = timesheetConfigSection.DisplayLeader;
    }
}