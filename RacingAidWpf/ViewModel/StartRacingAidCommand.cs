using System.Windows.Input;
using RacingAidData;
using RacingAidWpf.View;

namespace RacingAidWpf.ViewModel;

public class StartRacingAidCommand(RacingAid racingAid) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        racingAid.Start();
        
        TelemetryWindow telemetryWindow = new TelemetryWindow();
        telemetryWindow.Show();
    }
}