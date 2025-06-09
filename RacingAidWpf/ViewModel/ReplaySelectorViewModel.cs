using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using RacingAidWpf.Commands;

namespace RacingAidWpf.ViewModel;

public class ReplaySelectorViewModel : ViewModel
{
    private Dictionary<string, string> replayFileMap = new();
    
    public event Action<string> ReplayFileSelected;

    public event Action CloseRequested;

    private List<string> replayFilePaths;

    public List<string> ReplayFilePaths
    {
        get => replayFilePaths;
        set
        {
            if (replayFilePaths == value)
                return;
            
            replayFilePaths = value;
            replayFileMap = replayFilePaths.ToDictionary(Path.GetFileName, f => f);
            
            ReplayFiles = new ObservableCollection<string>(replayFileMap.Keys);
            OnPropertyChanged(nameof(ReplayFiles));
        }
    }
    public ObservableCollection<string> ReplayFiles { get; set; }
    
    private string selectedReplayFile;
    public string SelectedReplayFile
    {
        get => selectedReplayFile;
        set
        {
            if (selectedReplayFile == value)
                return;
            
            selectedReplayFile = value;
            OnPropertyChanged();
        }
    }
    
    public ICommand SelectReplayFileCommand { get; }
    
    public ICommand CloseCommand { get; }

    public ReplaySelectorViewModel()
    {
        SelectReplayFileCommand = new Command(SelectReplayFile);
        CloseCommand = new Command(Close);
    }

    private void SelectReplayFile()
    {
        if (replayFileMap.TryGetValue(SelectedReplayFile, out var replayFilePath))
            ReplayFileSelected?.Invoke(replayFilePath);
        
        Close();
    }

    private void Close()
    {
        CloseRequested?.Invoke();
    }
}