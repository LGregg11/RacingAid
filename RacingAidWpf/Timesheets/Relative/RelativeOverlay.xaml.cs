namespace RacingAidWpf.Timesheets.Relative;

public partial class RelativeOverlay
{
    private readonly RelativeOverlayViewModel relativeOverlayViewModel = new();
    
    public RelativeOverlay()
    {
        InitializeComponent();
        DataContext = relativeOverlayViewModel;
    }
}