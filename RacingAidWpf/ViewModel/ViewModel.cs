using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace RacingAidWpf.ViewModel;

public abstract class ViewModel : INotifyPropertyChanged
{
    private Dispatcher Dispatcher => Application.Current.Dispatcher;
    
    public event PropertyChangedEventHandler PropertyChanged;
    

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void InvokeOnMainThread(Action action)
    {
        Dispatcher.Invoke(action);
    }
}