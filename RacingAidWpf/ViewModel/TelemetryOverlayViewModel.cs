using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts;
using LiveCharts.Wpf;
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
    
    public double ChartWidth { get; set; } = 100;
    private int NChartPoints => (int)(ChartWidth / 2);
    public SolidColorBrush ThrottleBrush { get; } = Brushes.Green;
    public SolidColorBrush BrakeBrush { get; } = Brushes.Red;
    public SolidColorBrush ClutchBrush { get; } = Brushes.Blue;

    public SeriesCollection InputSeries { get; private init; }

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

        var clutchLineSeries = CreateLineSeries(ClutchBrush);
        var brakeLineSeries = CreateLineSeries(BrakeBrush);
        var throttleLineSeries = CreateLineSeries(ThrottleBrush);

        InputSeries = [clutchLineSeries, brakeLineSeries, throttleLineSeries];
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
        
        UpdateSeries(ThrottlePercentage, BrakePercentage, ClutchPercentage);
    }

    private void OnConfigUpdated()
    {
        // Force trigger a speed property change to update the units used in the view
        OnPropertyChanged(nameof(SpeedMetresPerSecond));
    }

    private void UpdateSeries(float throttle, float brake, float clutch)
    {
        InvokeOnMainThread(() =>
        {
            InputSeries[0].Values = UpdateInputPlot(InputSeries[0].Values, clutch);
            InputSeries[1].Values = UpdateInputPlot(InputSeries[1].Values, brake);
            InputSeries[2].Values = UpdateInputPlot(InputSeries[2].Values, throttle);
        });
        
        OnPropertyChanged(nameof(InputSeries));
    }

    private IChartValues UpdateInputPlot(IChartValues inputValues, float newValue)
    {
        // Remove the left-most data
        if (inputValues.Count >= NChartPoints)
            inputValues.RemoveAt(0);
        
        // Append to right of the plot - push values left
        inputValues.Add(newValue);
        
        return inputValues;
    }

    private LineSeries CreateLineSeries(SolidColorBrush brush)
    {
        var lineSeries = new LineSeries
        {
            Values = new ChartValues<float>(),
            Stroke = brush,
            StrokeThickness = 2f,
            Fill = Brushes.Transparent,
            PointGeometry = null,
            ToolTip = null
        };
        
        for (var i = 0; i < NChartPoints; i++)
            lineSeries.Values.Add(0f);

        return lineSeries;
    }
}