using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class TrackMapOverlay
{
    private readonly TrackMapOverlayViewModel trackMapOverlayViewModel = new();
    
    public TrackMapOverlay()
    {
        InitializeComponent();
        DataContext = trackMapOverlayViewModel;
    }
}