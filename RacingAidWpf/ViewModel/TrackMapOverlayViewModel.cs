using System.Windows;
using RacingAidData.Core.Models;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Tracks;

namespace RacingAidWpf.ViewModel;

public class TrackMapOverlayViewModel : ViewModel
{
    private static readonly DriverDataModel RacingAidDriverData = RacingAidSingleton.Instance.DriverData;
    private static readonly TrackDataModel RacingAidTrackData = RacingAidSingleton.Instance.TrackData;
    
    private readonly TrackMapController trackMapController;
    private readonly TrackMapCreator trackMapCreator;
    
    private readonly List<TrackMapPosition> currentTrackMapPositions = [];
    
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

        trackMapCreator.TrackCreated += OnTrackCreated;
        
        RacingAidUpdateDispatch.Update += OnUpdate;
    }

    private void OnUpdate()
    {
        CurrentTrackName = RacingAidTrackData.TrackName;

        if (CurrentTrackMap != null)
            UpdateDriverPositionsOnTrack();

        if (trackMapCreator.IsStarted)
            trackMapCreator.Update(RacingAidDriverData);
    }

    private void OnTrackChanged()
    {
        trackMapCreator.Stop();
        
        if (trackMapController.TryGetTrackMap(CurrentTrackName, out var trackMap))
        {
            CurrentTrackMap = trackMap;
            return;
        }
        
        CurrentTrackMap = null;
        trackMapCreator.Start(RacingAidDriverData, RacingAidTrackData);
    }

    private void OnTrackCreated(TrackMap trackMap)
    {
        trackMapController.AddTrackMap(trackMap);
        CurrentTrackMap = trackMap;
    }

    private void UpdateDriverPositionsOnTrack()
    {
        // TODO
    }
}