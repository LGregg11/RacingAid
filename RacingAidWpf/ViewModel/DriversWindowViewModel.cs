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
        var testDrivers = new ObservableCollection<DriverModel>
        {
            new()
            {
                FullName = "Test Driver1",
                CarModel = "Test Model1",
                CarNumber = 1,
                SafetyRating = "1234",
                SkillRating = "1234"
            },
            new()
            {
                FullName = "Test Driver2",
                CarModel = "Test Model2",
                CarNumber = 2,
                SafetyRating = "1345",
                SkillRating = "1345"
            },
            new()
            {
                FullName = "Test Driver3",
                CarModel = "Test Model3",
                CarNumber = 3,
                SafetyRating = "1456",
                SkillRating = "1456"
            }
        };

        Drivers = testDrivers;
    }
}