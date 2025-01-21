using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class TelemetryOverlay
{
    private readonly TelemetryOverlayViewModel telemetryOverlayViewModel = new();
    
    public TelemetryOverlay()
    {
        InitializeComponent();
        DataContext = telemetryOverlayViewModel;
    }
}