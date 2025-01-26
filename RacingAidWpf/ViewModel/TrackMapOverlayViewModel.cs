using System.Windows;
using System.Windows.Media;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Tracks;

namespace RacingAidWpf.ViewModel;

public class TrackMapOverlayViewModel : ViewModel
{
    private const float TargetSize = 300f;
    
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

    private GeometryGroup trackMapPathData;
    public GeometryGroup TrackMapPathData
    {
        get => trackMapPathData;
        set
        {
            if (trackMapPathData == value)
                return;
            
            trackMapPathData = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTrackMapAvailable));
            OnPropertyChanged(nameof(TrackMapVisibility));
            OnPropertyChanged(nameof(NoTrackMapTextVisibility));
        }
    }
    
    public bool IsTrackMapAvailable => TrackMapPathData != null;

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

        if (TrackMapPathData != null)
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
            UpdateTrackMapPathData(trackMap);
            return;
        }
        
        TrackMapPathData = null;
        trackMapCreator.Start(RacingAidSingleton.Instance.DriverData, RacingAidSingleton.Instance.TrackData);
    }

    private void OnTrackCreated(TrackMap trackMap)
    {
        Console.WriteLine($"Track created: {trackMap.Name}");
        
        trackMapController.AddTrackMap(trackMap);
        UpdateTrackMapPathData(trackMap);
    }

    private void UpdateDriverPositionsOnTrack()
    {
        // TODO
    }

    private void UpdateTrackMapPathData(TrackMap trackMap)
    {
        TrackMapPathData = TrackMapPathCreator.CreateGeometryGroupFromTrackMap(trackMap, TargetSize);
    }
}