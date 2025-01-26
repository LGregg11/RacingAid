using System.Windows;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Tracks;

namespace RacingAidWpf.ViewModel;

public class TrackMapOverlayViewModel : ViewModel
{
    private readonly TrackMapController trackMapController;
    
    private readonly List<TrackMapPosition> currentTrackMapPositions = [];

    private bool isCurrentlyTrackingMap;
    
    private string currentTrackName;
    private string CurrentTrackName
    {
        get => currentTrackName;
        set
        {
            if (currentTrackName == value)
                return;
            
            currentTrackName = value;
            OnTrackUpdated();
        }
    }

    private TrackMap currentTrackMap;
    private TrackMap CurrentTrackMap
    {
        get => currentTrackMap;
        set
        {
            if (currentTrackMap == value)
                return;
            
            currentTrackMap = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTrackMapAvailable));
            OnPropertyChanged(nameof(TrackMapVisibility));
            OnPropertyChanged(nameof(NoTrackMapTextVisibility));
        }
    }
    
    public bool IsTrackMapAvailable => CurrentTrackMap != null;

    public Visibility TrackMapVisibility => IsTrackMapAvailable ? Visibility.Visible : Visibility.Collapsed;
    public Visibility NoTrackMapTextVisibility => !IsTrackMapAvailable ? Visibility.Visible : Visibility.Collapsed;

    public TrackMapOverlayViewModel(TrackMapController trackMapController = null)
    {
        this.trackMapController = trackMapController ?? new TrackMapController(new JsonHandler<TrackMaps>());
        
        RacingAidUpdateDispatch.Update += OnUpdate;
    }

    private void OnUpdate()
    {
        CurrentTrackName = RacingAidSingleton.Instance.TrackData.TrackName;

        if (CurrentTrackMap != null)
        {
            UpdateDriverPositionsOnTrack();
            return;
        }

        if (isCurrentlyTrackingMap)
            UpdateCurrentTrackMapPositions();
    }

    private void OnTrackUpdated()
    {
        if (trackMapController.TryGetTrackMap(CurrentTrackName, out var trackMap))
        {
            CurrentTrackMap = trackMap;
            return;
        }
        
        CurrentTrackMap = null;
    }

    private void UpdateDriverPositionsOnTrack()
    {
        // TODO
    }
    
    private void UpdateCurrentTrackMapPositions()
    {
        // TODO
    }
}