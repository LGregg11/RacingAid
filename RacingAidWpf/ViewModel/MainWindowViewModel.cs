using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using RacingAidData.Simulators;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public sealed class MainWindowViewModel : NotifyPropertyChanged
{
    public ICommand StartRacingAidCommand { get; }

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
    
    public MainWindowViewModel()
    {
        var simulatorEntries = new List<SimulatorEntryModel>
        {
            new(Enum.GetName(Simulator.iRacing), Simulator.iRacing),
            new(Enum.GetName(Simulator.F1), Simulator.F1)
        };

        SimulatorEntryCollection = new ObservableCollection<SimulatorEntryModel>(simulatorEntries);
        SelectedSimulatorEntry = simulatorEntries.First();
        
        StartRacingAidCommand = new StartRacingAidCommand();
    }

    public void Start()
    {
        RacingAidSingleton.Instance.SetupSimulator(SelectedSimulatorEntry.SimulatorType);
        RacingAidSingleton.Instance.Start();
    }
}