using System.Collections.ObjectModel;
using RacingAidData.Core.Models;

namespace RacingAidWpf.ViewModel;

public class DriversWindowViewModel : NotifyPropertyChanged
{
    private ObservableCollection<DriverModel> drivers = [];
    public ObservableCollection<DriverModel> Drivers
    {
        get => drivers;
        private set
        {
            if (drivers == value)
                return;
            
            drivers = value;
            OnPropertyChanged();
        }
    }

    public DriversWindowViewModel()
    {
        RacingAidUpdateDispatch.Update += UpdateProperties;
    }

    private void UpdateProperties()
    {
        Drivers = new ObservableCollection<DriverModel>(RacingAidSingleton.Instance.Drivers.Drivers);
    }
}