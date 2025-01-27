using System.Collections.ObjectModel;
using System.Windows.Input;
using RacingAidData.Simulators;
using RacingAidWpf.Commands;
using RacingAidWpf.Configuration;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Model;
using RacingAidWpf.Overlays;
using RacingAidWpf.View;

namespace RacingAidWpf.ViewModel;

public sealed class MainWindowViewModel : ViewModel
{
    private readonly GeneralConfigSection generalConfigSection = ConfigSectionSingleton.GeneralSection;
    private readonly TimesheetConfigSection timesheetConfigSection = ConfigSectionSingleton.TimesheetSection;
    private readonly RelativeConfigSection relativeConfigSection = ConfigSectionSingleton.RelativeSection;
    private readonly TelemetryConfigSection telemetryConfigSection = ConfigSectionSingleton.TelemetrySection;
    private readonly OverlayController overlayController;

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }

    private bool isStarted;
    public bool IsStarted
    {
        get => isStarted;
        private set
        {
            if (isStarted == value)
                return;
            
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

    #region General

    public int UpdateIntervalMs
    {
        get => generalConfigSection.UpdateIntervalMs;
        set
        {
            if (generalConfigSection.UpdateIntervalMs == value)
                return;

            generalConfigSection.UpdateIntervalMs = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Timesheet
    
    public int TimesheetPositions
    {
        get => timesheetConfigSection.MaxPositions;
        set
        {
            if (timesheetConfigSection.MaxPositions == value)
                return;
            
            timesheetConfigSection.MaxPositions = value;
            OnPropertyChanged();
        }
    }
    
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

    #region Relative
    
    public int RelativePositions
    {
        get => relativeConfigSection.MaxPositionsAheadOrBehind;
        set
        {
            if (relativeConfigSection.MaxPositionsAheadOrBehind == value)
                return;
            
            relativeConfigSection.MaxPositionsAheadOrBehind = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplayRelativeCarNumber
    {
        get => relativeConfigSection.DisplayCarNumber;
        set
        {
            if (relativeConfigSection.DisplayCarNumber == value)
                return;

            relativeConfigSection.DisplayCarNumber = value;
            OnPropertyChanged();
        }
    }

    public bool DisplayRelativeSafetyRating
    {
        get => relativeConfigSection.DisplaySafetyRating;
        set
        {
            if (relativeConfigSection.DisplaySafetyRating == value)
                return;

            relativeConfigSection.DisplaySafetyRating = value;
            OnPropertyChanged();
        }
    }

    public bool DisplayRelativeSkillRating
    {
        get => relativeConfigSection.DisplaySkillRating;
        set
        {
            if (relativeConfigSection.DisplaySkillRating == value)
                return;

            relativeConfigSection.DisplaySkillRating = value;
            OnPropertyChanged();
        }
    }

    public bool DisplayRelativeLastLap
    {
        get => relativeConfigSection.DisplayLastLap;
        set
        {
            if (relativeConfigSection.DisplayLastLap == value)
                return;

            relativeConfigSection.DisplayLastLap = value;
            OnPropertyChanged();
        }
    }

    public bool DisplayRelativeFastestLap
    {
        get => relativeConfigSection.DisplayFastestLap;
        set
        {
            if (relativeConfigSection.DisplayFastestLap == value)
                return;

            relativeConfigSection.DisplayFastestLap = value;
            OnPropertyChanged();
        }
    }

    public bool DisplayRelativeDelta
    {
        get => relativeConfigSection.DisplayGapToLocal;
        set
        {
            if (relativeConfigSection.DisplayGapToLocal == value)
                return;

            relativeConfigSection.DisplayGapToLocal = value;
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

    #region Reposition Button Logic

    public bool IsRepositionEnabled
    {
        get => overlayController.IsRepositioningEnabled;
        set
        {
            if (overlayController.IsRepositioningEnabled == value)
                return;
            
            overlayController.IsRepositioningEnabled = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public MainWindowViewModel(OverlayController injectedOverlayController = null, List<Overlay> overlays = null)
    {
        overlayController = injectedOverlayController ?? new OverlayController(new JsonHandler<OverlayPositions>());
        overlays ??=
        [
            new TelemetryOverlay(),
            new TimesheetOverlay(),
            new RelativeOverlay(),
            new TrackMapOverlay()
        ];
        
        foreach (var overlay in overlays)
            overlayController.AddOverlay(overlay);

        var simulatorEntries = new List<SimulatorEntryModel>
        {
            new(Enum.GetName(Simulator.iRacing), Simulator.iRacing),
            new(Enum.GetName(Simulator.F1), Simulator.F1)
        };

        SimulatorEntryCollection = new ObservableCollection<SimulatorEntryModel>(simulatorEntries);
        SelectedSimulatorEntry = simulatorEntries.First();

        StartCommand = new Command(Start);
        StopCommand = new Command(Stop);
    }

    public void Close()
    {
        Stop();
        overlayController.CloseAll();
    }

    public void Start()
    {
        IsStarted = true;

        RacingAidSingleton.Instance.SetupSimulator(SelectedSimulatorEntry.SimulatorType);
        RacingAidSingleton.Instance.Start();

        RacingAidUpdateDispatch.Start();

        overlayController.ShowAll();
    }

    public void Stop()
    {
        RacingAidSingleton.Instance.Stop();
        RacingAidUpdateDispatch.Stop();

        IsRepositionEnabled = false;
        overlayController.HideAll();

        IsStarted = false;
    }

    public void ToggleOverlayRepositioning()
    {
        if (IsStarted)
        {
            IsRepositionEnabled = !IsRepositionEnabled;
        }
    }
}
