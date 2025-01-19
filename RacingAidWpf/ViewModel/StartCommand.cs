using System.Windows.Input;
using RacingAidWpf.View;

namespace RacingAidWpf.ViewModel;

public class StartCommand(Action startAction) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        startAction();
    }
}