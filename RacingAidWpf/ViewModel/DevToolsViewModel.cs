using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using RacingAidData.Core.Replay;
using RacingAidWpf.Commands;
using RacingAidWpf.Core.Singleton;
using RacingAidWpf.Model;
using RacingAidWpf.View;

namespace RacingAidWpf.ViewModel;

public class DevToolsViewModel : ViewModel
{
    private readonly ReplayController replayController;

    public event Action ReplayStarted;
    public event Action ReplayStopped;
    
    public ICommand StartRecordingCommand { get; }
    public ICommand StopRecordingCommand { get; }
    
    public ICommand OpenReplaySelectorCommand { get; }
    public ICommand StartReplayCommand { get; }
    public ICommand StopReplayCommand { get; }
    
    public string SelectedReplayFileName => Path.GetFileName(SelectedReplayFilePath);

    private string selectedReplayFilePath;

    public string SelectedReplayFilePath
    {
        get => selectedReplayFilePath;
        private set
        {
            if (selectedReplayFilePath == value)
                return;
            
            selectedReplayFilePath = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedReplayFileName));
        }
    }
    
    public ObservableCollection<EnumEntryModel<ReplaySpeed>> ReplaySpeedEntries { get; }

    private EnumEntryModel<ReplaySpeed> replaySpeedEntry;
    public EnumEntryModel<ReplaySpeed> ReplaySpeedEntry
    {
        get => replaySpeedEntry;
        set
        {
            if (replaySpeedEntry == value)
                return;

            replaySpeedEntry = ReplaySpeedEntries.First(d => d == value);
            replayController.ReplaySpeed = replaySpeedEntry.Value;
            OnPropertyChanged();
        }
    }
    
    public DevToolsViewModel()
    {
        // Setup recording
        replayController = new ReplayController();
        
        var racingAid = RacingAidSingleton.Instance;
        racingAid.SetupReplayController(replayController);
        
        ReplaySpeedEntries = EnumEntryModelUtility.CreateObservableEnumCollection<ReplaySpeed>();
        ReplaySpeedEntry = ReplaySpeedEntries.First();
        
        StartRecordingCommand = new Command(StartRecording);
        StopRecordingCommand = new Command(StopRecording);

        OpenReplaySelectorCommand = new Command(OpenReplaySelector);
        StartReplayCommand = new Command(StartReplay);
        StopReplayCommand = new Command(StopReplay);
    }

    private void StartRecording()
    {
        Logger?.LogInformation("Starting recording");
        var recordFile = replayController.StartRecording();
        Logger?.LogDebug($"Started recording: {recordFile}");
    }

    private void StopRecording()
    {
        Logger?.LogInformation("Stopping recording");
        replayController.StopRecording();
        Logger?.LogDebug("Stopped recording");
    }

    private void OpenReplaySelector()
    {
        var replaySelectorView = new ReplaySelectorView();
        if (replaySelectorView.DataContext is ReplaySelectorViewModel viewModel)
        {
            viewModel.ReplayFileSelected += OnReplayFileSelected;
            viewModel.ReplayFilePaths = [..replayController.GetReplays()];
        }

        replaySelectorView.ShowDialog();
    }

    private void OnReplayFileSelected(string replayFilePath)
    {
        if (!replayController.SelectReplay(replayFilePath))
            return;
        
        SelectedReplayFilePath = replayFilePath;
        Logger?.LogDebug($"Replay selected: {SelectedReplayFilePath}");
    }

    private void StartReplay()
    {
        Logger?.LogInformation("Starting replay");
        replayController.StartReplay();
        ReplayStarted?.Invoke();
        Logger?.LogDebug("Started replay");
    }

    private void StopReplay()
    {
        Logger?.LogInformation("Stopping replay");
        replayController.StopReplay();
        ReplayStopped?.Invoke();
        Logger?.LogDebug("Stopped replay");
    }
}