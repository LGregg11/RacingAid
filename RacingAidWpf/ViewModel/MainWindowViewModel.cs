using System.Collections.ObjectModel;
using System.Windows.Input;
using RacingAidData.Core.Replay;
using RacingAidData.Simulators;
using RacingAidWpf.Commands;
using RacingAidWpf.Core.Configuration;
using RacingAidWpf.Core.Dispatchers;
using RacingAidWpf.Core.FileHandlers;
using RacingAidWpf.Core.Logging;
using RacingAidWpf.Core.Overlays;
using RacingAidWpf.Core.Singleton;
using RacingAidWpf.Core.Telemetry;
using RacingAidWpf.Core.Timesheets.Leaderboard;
using RacingAidWpf.Core.Timesheets.Relative;
using RacingAidWpf.Core.Tracks;
using RacingAidWpf.Model;
using RacingAidWpf.View;

namespace RacingAidWpf.ViewModel;

public sealed class MainWindowViewModel : ViewModel
{
    private readonly GeneralConfigSection generalConfigSection = ConfigSectionSingleton.GeneralSection;
    private readonly LeaderboardConfigSection leaderboardConfigSection = ConfigSectionSingleton.LeaderboardSection;
    private readonly RelativeConfigSection relativeConfigSection = ConfigSectionSingleton.RelativeSection;
    private readonly TelemetryConfigSection telemetryConfigSection = ConfigSectionSingleton.TelemetrySection;
    private readonly TrackMapConfigSection trackMapConfigSection = ConfigSectionSingleton.TrackMapSection;
    private readonly OverlayController overlayController;
    private readonly ReplayController replayController;

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    
    public ICommand StartRecordingCommand { get; }
    public ICommand StopRecordingCommand { get; }

    private bool inSession;
    public bool InSession
    {
        get => inSession;
        private set
        {
            if (inSession == value)
                return;
            
            inSession = value;
            OnPropertyChanged();
            
            OnStartedOrSessionUpdate();
        }
    }

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

            OnStartedOrSessionUpdate();
        }
    }

    public bool IsStopped => !IsStarted;

    public ObservableCollection<EnumEntryModel<Simulator>> SimulatorEntries { get; }

    private EnumEntryModel<Simulator> selectedSimulatorEntry;
    public EnumEntryModel<Simulator> SelectedSimulatorEntry
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

    #region Leaderboard

    public bool IsLeaderboardOverlayEnabled
    {
        get => leaderboardConfigSection.IsEnabled;
        set
        {
            if (leaderboardConfigSection.IsEnabled == value)
                return;

            leaderboardConfigSection.IsEnabled = value;
            OnPropertyChanged();

            if (!overlayController.Exists<LeaderboardOverlay>())
                return;
            
            if (leaderboardConfigSection.IsEnabled)
                overlayController.EnableOverlayOfType<LeaderboardOverlay>();
            else
                overlayController.DisableOverlayOfType<LeaderboardOverlay>();
        }
    }
    
    public int LeaderboardPositions
    {
        get => leaderboardConfigSection.MaxPositions;
        set
        {
            if (leaderboardConfigSection.MaxPositions == value)
                return;
            
            leaderboardConfigSection.MaxPositions = value;
            OnPropertyChanged();
        }
    }
    
    public bool DisplayCarNumber
    {
        get => leaderboardConfigSection.DisplayCarNumber;
        set
        {
            if (leaderboardConfigSection.DisplayCarNumber == value)
                return;

            leaderboardConfigSection.DisplayCarNumber = value;
            OnPropertyChanged();
        }
    }

    public bool DisplaySafetyRating
    {
        get => leaderboardConfigSection.DisplaySafetyRating;
        set
        {
            if (leaderboardConfigSection.DisplaySafetyRating == value)
                return;

            leaderboardConfigSection.DisplaySafetyRating = value;
            OnPropertyChanged();
        }
    }

    public bool DisplaySkillRating
    {
        get => leaderboardConfigSection.DisplaySkillRating;
        set
        {
            if (leaderboardConfigSection.DisplaySkillRating == value)
                return;

            leaderboardConfigSection.DisplaySkillRating = value;
            OnPropertyChanged();
        }
    }

    public bool DisplayLastLap
    {
        get => leaderboardConfigSection.DisplayLastLap;
        set
        {
            if (leaderboardConfigSection.DisplayLastLap == value)
                return;

            leaderboardConfigSection.DisplayLastLap = value;
            OnPropertyChanged();
        }
    }

    public bool DisplayFastestLap
    {
        get => leaderboardConfigSection.DisplayFastestLap;
        set
        {
            if (leaderboardConfigSection.DisplayFastestLap == value)
                return;

            leaderboardConfigSection.DisplayFastestLap = value;
            OnPropertyChanged();
        }
    }

    public bool DisplayGapToLeader
    {
        get => leaderboardConfigSection.DisplayGapToLeader;
        set
        {
            if (leaderboardConfigSection.DisplayGapToLeader == value)
                return;

            leaderboardConfigSection.DisplayGapToLeader = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Relative

    public bool IsRelativeOverlayEnabled
    {
        get => relativeConfigSection.IsEnabled;
        set
        {
            if (relativeConfigSection.IsEnabled == value)
                return;

            relativeConfigSection.IsEnabled = value;
            OnPropertyChanged();

            if (!overlayController.Exists<RelativeOverlay>())
                return;
            
            if (relativeConfigSection.IsEnabled)
                overlayController.EnableOverlayOfType<RelativeOverlay>();
            else
                overlayController.DisableOverlayOfType<RelativeOverlay>();
        }
    }
    
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

    public bool IsTelemetryOverlayEnabled
    {
        get => telemetryConfigSection.IsEnabled;
        set
        {
            if (telemetryConfigSection.IsEnabled == value)
                return;

            telemetryConfigSection.IsEnabled = value;
            OnPropertyChanged();

            if (!overlayController.Exists<TelemetryOverlay>())
                return;
            
            if (telemetryConfigSection.IsEnabled)
                overlayController.EnableOverlayOfType<TelemetryOverlay>();
            else
                overlayController.DisableOverlayOfType<TelemetryOverlay>();
        }
    }

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

    #region Track Map

    public bool IsTrackMapOverlayEnabled
    {
        get => trackMapConfigSection.IsEnabled;
        set
        {
            if (trackMapConfigSection.IsEnabled == value)
                return;

            trackMapConfigSection.IsEnabled = value;
            OnPropertyChanged();

            if (!overlayController.Exists<TrackMapOverlay>())
                return;
            
            if (trackMapConfigSection.IsEnabled)
                overlayController.EnableOverlayOfType<TrackMapOverlay>();
            else
                overlayController.DisableOverlayOfType<TrackMapOverlay>();
        }
    }
    
    public ObservableCollection<EnumEntryModel<DriverNumberType>> DriverNumberEntries { get; }

    private EnumEntryModel<DriverNumberType> selectedDriverNumberEntry;
    public EnumEntryModel<DriverNumberType> SelectedDriverNumberEntry
    {
        get => selectedDriverNumberEntry;
        set
        {
            if (selectedDriverNumberEntry == value)
                return;

            selectedDriverNumberEntry = DriverNumberEntries.First(d => d == value);
            trackMapConfigSection.DriverNumberType = value.Value;
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

    public MainWindowViewModel(OverlayController overlayController = null, List<Overlay> overlays = null, ILogger logger = null)
    {
        Logger = logger ?? LoggerFactory.GetLogger<MainWindowViewModel>();
        this.overlayController = overlayController ?? new OverlayController(new JsonHandler<OverlayPositions>());
        overlays ??=
        [
            new TelemetryOverlay { IsOverlayEnabled = telemetryConfigSection.IsEnabled },
            new LeaderboardOverlay { IsOverlayEnabled = leaderboardConfigSection.IsEnabled },
            new RelativeOverlay { IsOverlayEnabled = relativeConfigSection.IsEnabled },
            new TrackMapOverlay { IsOverlayEnabled = trackMapConfigSection.IsEnabled }
        ];

        Logger?.LogDebug("Creating overlays");
        foreach (var overlay in overlays)
            this.overlayController.AddOverlay(overlay);

        Logger?.LogDebug("Creating simulator entries");
        SimulatorEntries = CreateObservableEnumCollection<Simulator>();
        SelectedSimulatorEntry = SimulatorEntries.First();
        
        Logger?.LogDebug("Creating track map driver number entries");
        DriverNumberEntries = CreateObservableEnumCollection<DriverNumberType>();
        SelectedDriverNumberEntry = DriverNumberEntries.First(d => d.Value == trackMapConfigSection.DriverNumberType);
        
        // Setup recording
        replayController = new ReplayController();
        
        var racingAid = RacingAidSingleton.Instance;
        racingAid.SetupReplayController(replayController);

        StartCommand = new Command(Start);
        StopCommand = new Command(Stop);
        
        StartRecordingCommand = new Command(StartRecording);
        StopRecordingCommand = new Command(StopRecording);
    }

    public void Close()
    {
        Logger?.LogDebug("Stopping and closing overlays");
        Stop();
        overlayController.CloseAll();
        Logger?.LogDebug("Overlays closed");
    }

    public void Start()
    {
        Logger?.LogDebug("Starting racing aid");
        IsStarted = true;

        var racingAid = RacingAidSingleton.Instance;

        racingAid.SetupSimulator(SelectedSimulatorEntry.Value);
        racingAid.InSessionUpdated += OnSessionUpdated;
        racingAid.Start();
        
        InSession = racingAid.InSession;
        Logger?.LogInformation("Started racing aid");
    }

    public void StartRecording()
    {
        var recordFile = replayController.StartRecording();
        Logger?.LogDebug($"Starting recording: {recordFile}");
    }

    public void StopRecording()
    {
        replayController.StopRecording();
        Logger?.LogDebug("Stopping recording");
    }

    public void Stop()
    {
        Logger?.LogDebug("Stopping racing aid");
        var racingAid = RacingAidSingleton.Instance;
        racingAid.Stop();
        racingAid.InSessionUpdated -= OnSessionUpdated;

        IsStarted = false;
        InSession = false;
        Logger?.LogInformation("Stopped racing aid");
    }

    public void ToggleOverlayRepositioning()
    {
        if (!IsStarted)
        {
            Logger?.LogWarning($"Tried to toggle {nameof(IsRepositionEnabled)} when app has not started");
            return;
        }
        
        IsRepositionEnabled = !IsRepositionEnabled;
    }

    private void OnSessionUpdated(bool connected)
    {
        var sessionStatus = connected ? "Started" : "Left";
        Logger?.LogDebug($"Session {sessionStatus}");
        
        // Make sure this is done on the main thread
        InvokeOnMainThread(() => { InSession = connected; });
    }

    private void OnStartedOrSessionUpdate()
    {
        var startedAndConnected = IsStarted && InSession;
        
        if (startedAndConnected)
            StartAndDisplayOverlays();
        else
            HideAndResetOverlays();
    }

    private void StartAndDisplayOverlays()
    {
        Logger?.LogDebug("Activating overlays");
        RacingAidUpdateDispatch.Start();
        overlayController.AreOverlaysActive = true;
    }

    private void HideAndResetOverlays()
    {
        Logger?.LogDebug("Stopping updates & disabling repositioning");
        RacingAidUpdateDispatch.Stop();
        IsRepositionEnabled = false;
        
        Logger?.LogDebug("Deactivating & Resetting overlays");
        overlayController.AreOverlaysActive = false;
        overlayController.ResetAll();
    }

    private static ObservableCollection<EnumEntryModel<T>> CreateObservableEnumCollection<T>() where T : struct, Enum
    {
        var entries = new ObservableCollection<EnumEntryModel<T>>();
        foreach (var entry in Enum.GetValues(typeof(T)).Cast<T>())
            entries.Add(new EnumEntryModel<T>(entry));
        return entries;
    }
}
