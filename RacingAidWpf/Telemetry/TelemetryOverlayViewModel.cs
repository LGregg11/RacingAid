using System.Windows.Media.Imaging;
using RacingAidData;
using RacingAidWpf.Configuration;
using RacingAidWpf.Dispatchers;
using RacingAidWpf.Logging;
using RacingAidWpf.Overlays;
using RacingAidWpf.Resources;
using RacingAidWpf.Singleton;

namespace RacingAidWpf.Telemetry;

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
            InvokeOnMainThread(() => OnPropertyChanged());
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
            InvokeOnMainThread(() => OnPropertyChanged());
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
            InvokeOnMainThread(() => OnPropertyChanged());
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
            InvokeOnMainThread(() => OnPropertyChanged());
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
            InvokeOnMainThread(() => OnPropertyChanged());
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
            InvokeOnMainThread(() => OnPropertyChanged());
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
            InvokeOnMainThread(() =>
            {
                OnPropertyChanged();
                OnPropertyChanged(nameof(SteeringWheelAngle));
            });
        }
    }

    public double SteeringWheelAngle => -1d * SteeringAngleDegrees;

    public BitmapImage SteeringWheelImage => new(Resource.SteeringWheelUri);

    public TelemetryOverlayViewModel(ILogger logger = null)
    {
        Logger = logger ?? LoggerFactory.GetLogger<TelemetryOverlayViewModel>();
        
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
        
        var fullName = racingAid.Leaderboard.LocalEntry?.FullName;
        if (!string.IsNullOrEmpty(fullName))
            DriverName = fullName;
    }

    private void OnConfigUpdated()
    {
        // Force trigger a speed property change to update the units used in the view
        OnPropertyChanged(nameof(SpeedMetresPerSecond));
    }
}