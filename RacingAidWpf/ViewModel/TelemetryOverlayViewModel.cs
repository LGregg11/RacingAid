using System.Windows.Media.Imaging;
using RacingAidData;
using RacingAidWpf.Configuration;
using RacingAidWpf.Resources;

namespace RacingAidWpf.ViewModel;

public class TelemetryOverlayViewModel : OverlayViewModel
{
    private const float FloatTolerance = 1e-4f;
    
    private readonly RacingAid racingAid = RacingAidSingleton.Instance;
    private readonly TelemetryConfigSection telemetryConfigSection = ConfigSectionSingleton.TelemetrySection;
    
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

    private float speedMetresPerSecond;
    public float SpeedMetresPerSecond
    {
        get => speedMetresPerSecond;
        private set
        {
            if (Math.Abs(speedMetresPerSecond - value) < FloatTolerance)
                return;
            
            speedMetresPerSecond = value;
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

    public TelemetryOverlayViewModel()
    {
        RacingAidUpdateDispatch.Update += UpdateProperties;
        
        telemetryConfigSection.ConfigUpdated += OnConfigUpdated;
    }

    public override void Reset()
    {
        // Do nothing
    }

    private void UpdateProperties()
    {
        var telemetry = racingAid.Telemetry;
        
        ThrottlePercentage = telemetry.ThrottleInput;
        BrakePercentage = telemetry.BrakeInput;
        ClutchPercentage = 1f - telemetry.ClutchInput;
        SpeedMetresPerSecond = telemetry.SpeedMs;
        Gear = telemetry.Gear;
        SteeringAngleDegrees = telemetry.SteeringAngleDegrees;
        
        var fullName = racingAid.Timesheet.LocalEntry?.FullName;
        if (!string.IsNullOrEmpty(fullName))
            DriverName = fullName;
    }

    private void OnConfigUpdated()
    {
        // Force trigger a speed property change to update the units used in the view
        OnPropertyChanged(nameof(SpeedMetresPerSecond));
    }
}