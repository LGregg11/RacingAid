using System.Windows;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class ReplaySelectorView : Window
{
    private readonly ReplaySelectorViewModel replaySelectorViewModel = new();
    
    public ReplaySelectorView()
    {
        InitializeComponent();
        
        DataContext = replaySelectorViewModel;
        replaySelectorViewModel.CloseRequested += Close;
    }
}