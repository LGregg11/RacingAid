using System.Windows.Input;
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

    private void TelemetryGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftCtrl))
            DragMove();
    }
}