using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class RelativeOverlay
{
    private readonly RelativeOverlayViewModel relativeOverlayViewModel = new();
    
    public RelativeOverlay()
    {
        InitializeComponent();
        DataContext = relativeOverlayViewModel;
    }
}