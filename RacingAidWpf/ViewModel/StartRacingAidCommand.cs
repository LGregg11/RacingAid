using System.Windows.Input;
using RacingAidWpf.View;

namespace RacingAidWpf.ViewModel;

public class StartRacingAidCommand() : ICommand
{
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        RacingAidSingleton.Instance.Start();
        
        TelemetryWindow telemetryWindow = new TelemetryWindow();
        telemetryWindow.Show();
        
        DriversWindow driversWindow = new DriversWindow();
        driversWindow.Show();
    }
}