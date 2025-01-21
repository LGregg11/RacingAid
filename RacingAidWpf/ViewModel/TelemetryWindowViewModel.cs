using System.Windows.Media.Imaging;
using RacingAidWpf.Extensions;
using RacingAidWpf.Resources;

namespace RacingAidWpf.ViewModel;

public class TelemetryWindowViewModel : NotifyPropertyChanged
{
    private string driverName = "N/A";
    public string DriverName
    {
        get => driverName;
        private set
        {
            if (driverName == value)
                return;
            
            driverName = value;
            OnPropertyChanged();
        }
    }

    private float speedKph;
    public float SpeedKph
    {
        get => speedKph;
        private set
        {
            if (Math.Abs(speedKph - value) < 0.1f)
                return;
            
            speedKph = value;
            OnPropertyChanged();
        }
    }
    
    private float brakePercentage;
    public float BrakePercentage
    {
        get => brakePercentage;
        private set
        {
            if (Math.Abs(brakePercentage - value) < 0.1f)
                return;
            
            brakePercentage = value;
            OnPropertyChanged();
        }
    }
    
    private float throttlePercentage;
    public float ThrottlePercentage
    {
        get => throttlePercentage;
        private set
        {
            if (Math.Abs(throttlePercentage - value) < 0.1f)
                return;
            
            throttlePercentage = value;
            OnPropertyChanged();
        }
    }
    
    private int gear;
    public int Gear
    {
        get => gear;
        private set
        {
            if (gear == value)
                return;
            
            gear = value;
            OnPropertyChanged();
        }
    }

    private float steeringAngleDegrees;

    public float SteeringAngleDegrees
    {
        get => steeringAngleDegrees;
        private set
        {
            if (Math.Abs(steeringAngleDegrees - value) < 0.1f)
                return;
            
            steeringAngleDegrees = value;
            OnPropertyChanged();
        }
    }

    public BitmapImage SteeringWheelImage => new(Resource.SteeringWheelUri);

    public TelemetryWindowViewModel()
    {
        RacingAidUpdateDispatch.Update += UpdateProperties;
    }
    
    private void UpdateProperties()
    {
        var telemetry = RacingAidSingleton.Instance.Telemetry;
        
        SpeedKph = telemetry.SpeedMetresPerSecond.ToKph();
        BrakePercentage = telemetry.BrakePercentage;
        ThrottlePercentage = telemetry.ThrottlePercentage;
        Gear = telemetry.Gear;
        SteeringAngleDegrees = telemetry.SteeringAngleDegrees;
        
        var fullName = RacingAidSingleton.Instance.Timesheet.LocalEntry?.FullName;
        if (!string.IsNullOrEmpty(fullName))
            DriverName = fullName;
    }
}