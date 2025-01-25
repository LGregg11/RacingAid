using System.Windows;

namespace RacingAidWpf.ViewModel;

public class TrackMapOverlayViewModel : ViewModel
{
    private bool isTrackMapAvailable;

    public bool IsTrackMapAvailable
    {
        get => isTrackMapAvailable;
        set
        {
            if (isTrackMapAvailable == value)
                return;
            
            isTrackMapAvailable = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TrackMapVisibility));
            OnPropertyChanged(nameof(NoTrackMapTextVisibility));
        }
    }

    public Visibility TrackMapVisibility => IsTrackMapAvailable ? Visibility.Visible : Visibility.Collapsed;
    public Visibility NoTrackMapTextVisibility => !IsTrackMapAvailable ? Visibility.Visible : Visibility.Collapsed;
}