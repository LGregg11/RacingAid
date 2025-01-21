using System.Windows.Media.Imaging;
using RacingAidWpf.Extensions;
using RacingAidWpf.Resources;

namespace RacingAidWpf.ViewModel;

public class TelemetryWindowViewModel : NotifyPropertyChanged
{
    private const float FloatTolerance = 0.01f;
    
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
            if (Math.Abs(speedKph - value) < FloatTolerance)
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
            if (Math.Abs(brakePercentage - value) < FloatTolerance)
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
            if (Math.Abs(throttlePercentage - value) < FloatTolerance)
                return;
            
            throttlePercentage = value;
            OnPropertyChanged();
        }
    }
    
    private float clutchPercentage;
    public float ClutchPercentage
    {
        get => clutchPercentage;
        private set
        {
            if (Math.Abs(clutchPercentage - value) < FloatTolerance)
                return;
            
            clutchPercentage = value;
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
            if (Math.Abs(steeringAngleDegrees - value) < FloatTolerance)
                return;
            
            steeringAngleDegrees = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SteeringWheelAngle));
        }
    }

    public double SteeringWheelAngle => -1d * SteeringAngleDegrees;

    public BitmapImage SteeringWheelImage => new(Resource.SteeringWheelUri);

    public TelemetryWindowViewModel()
    {
        RacingAidUpdateDispatch.Update += UpdateProperties;
    }
    
    private void UpdateProperties()
    {
        var telemetry = RacingAidSingleton.Instance.Telemetry;
        
        ThrottlePercentage = telemetry.ThrottleInput;
        BrakePercentage = telemetry.BrakeInput;
        ClutchPercentage = telemetry.ClutchInput;
        SpeedKph = telemetry.SpeedMetresPerSecond.ToKph();
        Gear = telemetry.Gear;
        SteeringAngleDegrees = telemetry.SteeringAngleDegrees;
        
        var fullName = RacingAidSingleton.Instance.Timesheet.LocalEntry?.FullName;
        if (!string.IsNullOrEmpty(fullName))
            DriverName = fullName;
    }
}