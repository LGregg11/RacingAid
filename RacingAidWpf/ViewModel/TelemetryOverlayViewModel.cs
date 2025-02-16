using System.Windows.Media.Imaging;
using RacingAidData;
using RacingAidWpf.Core.Configuration;
using RacingAidWpf.Core.Dispatchers;
using RacingAidWpf.Core.Logging;
using RacingAidWpf.Core.Overlays;
using RacingAidWpf.Core.Singleton;
using RacingAidWpf.Core.Telemetry;
using RacingAidWpf.Resources;

namespace RacingAidWpf.ViewModel;

public class TelemetryOverlayViewModel : OverlayViewModel
{
    private const float FloatTolerance = 1e-4f;
    
    private readonly RacingAid racingAid = RacingAidSingleton.Instance;
    private readonly TelemetryConfigSection telemetryConfigSection = ConfigSectionSingleton.TelemetrySection;
    private readonly TelemetryInfo telemetryInfo;
    
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

    public TelemetryOverlayViewModel(TelemetryInfo telemetryInfo = null, ILogger logger = null)
    {
        Logger = logger ?? LoggerFactory.GetLogger<TelemetryOverlayViewModel>();
        this.telemetryInfo = telemetryInfo ?? new TelemetryInfo();
        
        RacingAidUpdateDispatch.Update += UpdateProperties;
        
        telemetryConfigSection.ConfigUpdated += OnConfigUpdated;
    }

    public override void Reset()
    {
        Logger?.LogDebug("Resetting telemetry");
        telemetryInfo.Clear();
    }

    private void UpdateProperties()
    {
        telemetryInfo.UpdateFromData(racingAid.Telemetry);

        ThrottlePercentage = telemetryInfo.ThrottlePercentage;
        BrakePercentage = telemetryInfo.BrakePercentage;
        ClutchPercentage = telemetryInfo.ClutchPercentage;
        SpeedMetresPerSecond = telemetryInfo.SpeedMetresPerSecond;
        SteeringAngleDegrees = telemetryInfo.SteeringAngleDegrees;
        Gear = telemetryInfo.Gear;
        
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