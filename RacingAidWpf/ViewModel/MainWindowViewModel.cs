﻿using System.Collections.ObjectModel;
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
    
    private readonly GeneralConfigSection generalConfigSection = ConfigSectionSingleton.GeneralSection;
    private readonly TimesheetConfigSection timesheetConfigSection = ConfigSectionSingleton.TimesheetSection;
    private readonly TelemetryConfigSection telemetryConfigSection = ConfigSectionSingleton.TelemetrySection;
    
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
        get => timesheetConfigSection.DisplayCarNumber;
        set
        {
            if (timesheetConfigSection.DisplayCarNumber == value)
                return;
            
            timesheetConfigSection.DisplayCarNumber = value;
            OnPropertyChanged();
        }
    }

    public bool DisplaySafetyRating
    {
        get => timesheetConfigSection.DisplaySafetyRating;
        set
        {
            if (timesheetConfigSection.DisplaySafetyRating == value)
                return;
            
            timesheetConfigSection.DisplaySafetyRating = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplaySkillRating
    {
        get => timesheetConfigSection.DisplaySkillRating;
        set
        {
            if (timesheetConfigSection.DisplaySkillRating == value)
                return;
            
            timesheetConfigSection.DisplaySkillRating = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplayLastLap
    {
        get => timesheetConfigSection.DisplayLastLap;
        set
        {
            if (timesheetConfigSection.DisplayLastLap == value)
                return;
            
            timesheetConfigSection.DisplayLastLap = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplayFastestLap
    {
        get => timesheetConfigSection.DisplayFastestLap;
        set
        {
            if (timesheetConfigSection.DisplayFastestLap == value)
                return;
            
            timesheetConfigSection.DisplayFastestLap = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplayGapToLeader
    {
        get => timesheetConfigSection.DisplayGapToLeader;
        set
        {
            if (timesheetConfigSection.DisplayGapToLeader == value)
                return;
            
            timesheetConfigSection.DisplayGapToLeader = value;
            OnPropertyChanged();
        }
    }
    
    #endregion
    
    #region Telemetry
    
    public bool UseMetricUnits
    {
        get => telemetryConfigSection.UseMetricUnits;
        set
        {
            if (telemetryConfigSection.UseMetricUnits == value)
                return;
            
            telemetryConfigSection.UseMetricUnits = value;
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