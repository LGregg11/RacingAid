using System.Windows.Input;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class TimesheetOverlay
{
    private readonly TimesheetOverlayViewModel timesheetOverlayViewModel = new();
    
    public TimesheetOverlay()
    {
        InitializeComponent();
        DataContext = timesheetOverlayViewModel;
    }

    private void DriversGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftCtrl))
            DragMove();
    }
}