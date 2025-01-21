using System.Windows.Input;

namespace RacingAidWpf.ViewModel;

public class Command(Action action) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        action();
    }
}