namespace RacingAidWpf.Timesheets.Leaderboard;

public partial class LeaderboardOverlay
{
    private readonly LeaderboardOverlayViewModel leaderboardOverlayViewModel = new();
    
    public LeaderboardOverlay()
    {
        InitializeComponent();
        DataContext = leaderboardOverlayViewModel;
    }
}