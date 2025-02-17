using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class LeaderboardOverlay
{
    private readonly LeaderboardOverlayViewModel leaderboardOverlayViewModel = new();
    
    public LeaderboardOverlay()
    {
        InitializeComponent();
        DataContext = leaderboardOverlayViewModel;
    }
}