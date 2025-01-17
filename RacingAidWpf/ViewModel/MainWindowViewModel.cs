using System.Windows.Data;
using System.Windows.Input;
using RacingAidData;
using RacingAidData.Simulators;
using RacingAidWpf.Model;

namespace RacingAidWpf.ViewModel;

public sealed class MainWindowViewModel : NotifyPropertyChanged
{
    private readonly RacingAid racingAid = new();

    public ICommand StartRacingAidCommand { get; }

    public CollectionView SimulatorEntries { get; private set; }
    
    private SimulatorEntryModel selectedSimulatorEntry;

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
            new(Enum.GetName(Simulator.IRacing), Simulator.IRacing),
            new(Enum.GetName(Simulator.F1), Simulator.F1)
        };

        SimulatorEntries = new CollectionView(simulatorEntries);
        selectedSimulatorEntry = simulatorEntries.First();
        
        StartRacingAidCommand = new StartRacingAidCommand(racingAid);
    }

    public void Start()
    {
        racingAid.SetupSimulator(SelectedSimulatorEntry.SimulatorType);
        racingAid.Start();
    }
}