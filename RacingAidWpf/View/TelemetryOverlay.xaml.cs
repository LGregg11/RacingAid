using System.Windows.Input;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class TelemetryWindow
{
    private readonly TelemetryWindowViewModel telemetryWindowViewModel = new();
    
    public TelemetryWindow()
    {
        InitializeComponent();
        DataContext = telemetryWindowViewModel;
    }

    private void TelemetryGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftCtrl))
            DragMove();
    }
}