using System.Collections.ObjectModel;
using System.Windows.Input;
using RacingAidData.Simulators;
using RacingAidWpf.Configuration;
using RacingAidWpf.Model;
using RacingAidWpf.View;

namespace RacingAidWpf.ViewModel;

public sealed class MainWindowViewModel : NotifyPropertyChanged
{
    private TelemetryWindow? telemetryWindow;
    private TimesheetWindow? driversWindow;
    
    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }

    private bool isStarted;
    public bool IsStarted
    {
        get => isStarted;
        private set
        {
            isStarted = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsStopped));
        }
    }
    
    public bool IsStopped => !IsStarted;

    public ObservableCollection<SimulatorEntryModel> SimulatorEntryCollection { get; private set; }
    
    private SimulatorEntryModel selectedSimulatorEntry = new("N/A", Simulator.Unknown);

    public SimulatorEntryModel SelectedSimulatorEntry
    {
        get => selectedSimulatorEntry;
        set
        {
            if (selectedSimulatorEntry == value)
                return;

            selectedSimulatorEntry = value;
            OnPropertyChanged();
        }
    }
    
    #region Config properties
    
    #region Timesheet
    
    public bool DisplayCarNumber
    {
        get => ConfigSectionSingleton.TimesheetSection.DisplayCarNumber;
        set
        {
            if (ConfigSectionSingleton.TimesheetSection.DisplayCarNumber == value)
                return;
            
            ConfigSectionSingleton.TimesheetSection.DisplayCarNumber = value;
            OnPropertyChanged();
        }
    }

    public bool DisplaySafetyRating
    {
        get => ConfigSectionSingleton.TimesheetSection.DisplaySafetyRating;
        set
        {
            if (ConfigSectionSingleton.TimesheetSection.DisplaySafetyRating == value)
                return;
            
            ConfigSectionSingleton.TimesheetSection.DisplaySafetyRating = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplaySkillRating
    {
        get => ConfigSectionSingleton.TimesheetSection.DisplaySkillRating;
        set
        {
            if (ConfigSectionSingleton.TimesheetSection.DisplaySkillRating == value)
                return;
            
            ConfigSectionSingleton.TimesheetSection.DisplaySkillRating = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplayLastLap
    {
        get => ConfigSectionSingleton.TimesheetSection.DisplayLastLap;
        set
        {
            if (ConfigSectionSingleton.TimesheetSection.DisplayLastLap == value)
                return;
            
            ConfigSectionSingleton.TimesheetSection.DisplayLastLap = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplayFastestLap
    {
        get => ConfigSectionSingleton.TimesheetSection.DisplayFastestLap;
        set
        {
            if (ConfigSectionSingleton.TimesheetSection.DisplayFastestLap == value)
                return;
            
            ConfigSectionSingleton.TimesheetSection.DisplayFastestLap = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplayGapToLeader
    {
        get => ConfigSectionSingleton.TimesheetSection.DisplayGapToLeader;
        set
        {
            if (ConfigSectionSingleton.TimesheetSection.DisplayGapToLeader == value)
                return;
            
            ConfigSectionSingleton.TimesheetSection.DisplayGapToLeader = value;
            OnPropertyChanged();
        }
    }
    
    #endregion
    
    #region Telemetry
    
    public bool UseMetricUnits
    {
        get => ConfigSectionSingleton.TelemetrySection.UseMetricUnits;
        set
        {
            if (ConfigSectionSingleton.TelemetrySection.UseMetricUnits == value)
                return;
            
            ConfigSectionSingleton.TelemetrySection.UseMetricUnits = value;
            OnPropertyChanged();
        }
    }
    
    #endregion
    
    #endregion
    
    
    public MainWindowViewModel()
    {
        var simulatorEntries = new List<SimulatorEntryModel>
        {
            new(Enum.GetName(Simulator.iRacing), Simulator.iRacing),
            new(Enum.GetName(Simulator.F1), Simulator.F1)
        };

        SimulatorEntryCollection = new ObservableCollection<SimulatorEntryModel>(simulatorEntries);
        SelectedSimulatorEntry = simulatorEntries.First();
        
        StartCommand = new StartCommand(Start);
        StopCommand = new StopCommand(Stop);
    }

    private void Start()
    {
        IsStarted = true;
        
        RacingAidSingleton.Instance.SetupSimulator(SelectedSimulatorEntry.SimulatorType);
        RacingAidSingleton.Instance.Start();

        RacingAidUpdateDispatch.UpdateRefreshRateMs = 33;
        RacingAidUpdateDispatch.Start();
        
        telemetryWindow = new TelemetryWindow();
        telemetryWindow.Show();
        
        driversWindow = new TimesheetWindow();
        driversWindow.Show();
    }

    private void Stop()
    {
        RacingAidSingleton.Instance.Stop();
        RacingAidUpdateDispatch.Stop();
        
        telemetryWindow?.Close();
        driversWindow?.Close();

        IsStarted = false;
    }
}