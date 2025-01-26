using System.Windows;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Tracks;

namespace RacingAidWpf.ViewModel;

public class TrackMapOverlayViewModel : ViewModel
{
    private readonly TrackMapController trackMapController;
    private readonly TrackMapCreator trackMapCreator;
    
    private string currentTrackName;
    private string CurrentTrackName
    {
        get => currentTrackName;
        set
        {
            if (currentTrackName == value)
                return;
            
            currentTrackName = value;
            OnTrackChanged();
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

    public TrackMapOverlayViewModel(TrackMapController trackMapController = null, TrackMapCreator trackMapCreator = null)
    {
        this.trackMapController = trackMapController ?? new TrackMapController(new JsonHandler<TrackMaps>());
        this.trackMapCreator = trackMapCreator ?? new TrackMapCreator();

        this.trackMapCreator.TrackCreated += OnTrackCreated;
        
        RacingAidUpdateDispatch.Update += OnUpdate;
    }

    ~TrackMapOverlayViewModel()
    {
        RacingAidUpdateDispatch.Update -= OnUpdate;
        trackMapCreator.TrackCreated -= OnTrackCreated;
    }

    private void OnUpdate()
    {
        CurrentTrackName = RacingAidSingleton.Instance.TrackData.TrackName;

        if (CurrentTrackMap != null)
            UpdateDriverPositionsOnTrack();

        if (trackMapCreator.IsStarted)
            trackMapCreator.Update(RacingAidSingleton.Instance.DriverData);
    }

    private void OnTrackChanged()
    {
        trackMapCreator.Stop();

        if (string.IsNullOrEmpty(CurrentTrackName))
            return;
        
        if (trackMapController.TryGetTrackMap(CurrentTrackName, out var trackMap))
        {
            CurrentTrackMap = trackMap;
            return;
        }
        
        CurrentTrackMap = null;
        trackMapCreator.Start(RacingAidSingleton.Instance.DriverData, RacingAidSingleton.Instance.TrackData);
    }

    private void OnTrackCreated(TrackMap trackMap)
    {
        Console.WriteLine($"Track created: {trackMap.Name}");
        
        trackMapController.AddTrackMap(trackMap);
        CurrentTrackMap = trackMap;
    }

    private void UpdateDriverPositionsOnTrack()
    {
        // TODO
    }
}