using System.Collections.ObjectModel;
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
            OnPropertyChanged(nameof(TrackMapPathData));
            OnPropertyChanged(nameof(IsTrackMapAvailable));
            OnPropertyChanged(nameof(TrackMapVisibility));
            OnPropertyChanged(nameof(NoTrackMapTextVisibility));
        }
    }
    
    private ObservableCollection<DriverTrackVisualization> driverTrackVisualizations = [];
    public ObservableCollection<DriverTrackVisualization> DriverTrackVisualizations
    {
        get => driverTrackVisualizations;
        set
        {
            if (driverTrackVisualizations == value)
                return;
            
            driverTrackVisualizations = value;
            OnPropertyChanged();
        }
    }

    public GeometryGroup TrackMapPathData => CurrentTrackMap == null ? null : TrackMapPathCreator.CreateGeometryGroupFromTrackMap(CurrentTrackMap, TargetSize);
    
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
        var scaledPositions = TrackMapPathCreator.GetScaledTrackMapPositions(currentTrackMap, TargetSize);
        var nPositions = scaledPositions.Count;
        
        var visualizations = new ObservableCollection<DriverTrackVisualization>();
        
        foreach (var driver in RacingAidSingleton.Instance.Relative.Entries)
        {
            var lapsDriven = driver.LapsDriven;
            var lapPercentage = lapsDriven - (int)lapsDriven;

            var positionIndexRelativeToLapPercentage = (int)(lapPercentage * nPositions);
            var position = scaledPositions[positionIndexRelativeToLapPercentage];

            var fillColor = driver.IsLocal ? Brushes.Red : Brushes.LightGray;
            var borderColor = driver.IsLocal ? Brushes.WhiteSmoke : Brushes.LightGray;   
            var size = driver.IsLocal ? 20d : 15d;
            
            // TODO: update number based on number display type (overall, class, car number)
            var number = driver.OverallPosition;

            visualizations.Add(new DriverTrackVisualization(position.X, position.Y, size, number, fillColor, borderColor, 2d));
        }
        
        DriverTrackVisualizations = visualizations;
    }
}