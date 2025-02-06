namespace RacingAidWpf.Telemetry;

public partial class TelemetryOverlay
{
    private readonly TelemetryOverlayViewModel telemetryOverlayViewModel = new();
    
    public TelemetryOverlay()
    {
        InitializeComponent();
        DataContext = telemetryOverlayViewModel;
    }
}