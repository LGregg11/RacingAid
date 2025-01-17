using RacingAidData.Simulators;

namespace RacingAidWpf.Model;

public class SimulatorEntryModel(string? simulatorName, Simulator simulatorType)
{
    public string? SimulatorName { get; } = simulatorName;
    public Simulator SimulatorType { get; } = simulatorType;
}