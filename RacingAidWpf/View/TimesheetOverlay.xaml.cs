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
}