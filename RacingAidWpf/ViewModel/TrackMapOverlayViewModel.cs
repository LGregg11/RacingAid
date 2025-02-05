using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using RacingAidData.Core.Models;
using RacingAidWpf.Configuration;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Logging;
using RacingAidWpf.Tracks;

namespace RacingAidWpf.ViewModel;

public class TrackMapOverlayViewModel : OverlayViewModel
{
    private const float TargetSize = 300f;
    
    private readonly TrackMapConfigSection trackMapConfigSection = ConfigSectionSingleton.TrackMapSection;
    
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

            InvokeOnMainThread(() => OnPropertyChanged());
        }
    }

    public GeometryGroup TrackMapPathData => CurrentTrackMap == null ? null : TrackMapPathCreator.Create2DGeometryGroupFromTrackMap(CurrentTrackMap.Positions);
    
    public bool IsTrackMapAvailable => TrackMapPathData != null;

    public Visibility TrackMapVisibility => IsTrackMapAvailable ? Visibility.Visible : Visibility.Collapsed;
    public Visibility NoTrackMapTextVisibility => !IsTrackMapAvailable ? Visibility.Visible : Visibility.Collapsed;

    public TrackMapOverlayViewModel(TrackMapController trackMapController = null, TrackMapCreator trackMapCreator = null, ILogger logger = null)
    {
        Logger = logger ?? LoggerFactory.GetLogger<TrackMapOverlayViewModel>();
        
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

    public override void Reset()
    {
        Logger?.LogDebug($"Resetting {nameof(CurrentTrackName)}");
        CurrentTrackName = null;
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
        Logger?.LogDebug("Track changed");
        
        trackMapCreator.Stop();

        if (string.IsNullOrEmpty(CurrentTrackName))
        {
            Logger?.LogDebug($"{nameof(CurrentTrackName)} is null");
            CurrentTrackMap = null;
            return;
        }
        
        if (trackMapController.TryGetTrackMap(CurrentTrackName, out var trackMap))
        {
            Logger?.LogDebug($"Found existing track map data for '{CurrentTrackName}'");
            CurrentTrackMap = TransformTrackMap(trackMap);
            return;
        }
        
        Logger?.LogDebug($"No existing track map data for '{CurrentTrackName}' - start creation");
        CurrentTrackMap = null;
        trackMapCreator.Start(RacingAidSingleton.Instance.DriverData, RacingAidSingleton.Instance.TrackData);
    }

    private void OnTrackCreated(TrackMap trackMap)
    {
        Logger?.LogInformation($"Track map created for '{trackMap.Name}'");
        
        trackMapController.AddTrackMap(trackMap);
        CurrentTrackMap = TransformTrackMap(trackMap);
    }

    private void UpdateDriverPositionsOnTrack()
    {
        var driverNumberType = trackMapConfigSection.DriverNumberType;
        
        var visualizations = new ObservableCollection<DriverTrackVisualization>();
        foreach (var driver in RacingAidSingleton.Instance.Relative.Entries)
        {
            if (CreateDriverTrackVisualization(currentTrackMap.Positions, driver, driverNumberType) is {} visualization)
                visualizations.Add(visualization);
        }

        DriverTrackVisualizations = visualizations;
    }

    private static TrackMap TransformTrackMap(TrackMap trackMap)
    {
        // 0.95 for padding
        trackMap.Positions = TrackMapUtilities.UpdatePositions(
            trackMap.Positions,
            true,
            true,
            TargetSize * 0.95f);

        return trackMap;
    }

    private DriverTrackVisualization CreateDriverTrackVisualization(List<TrackMapPosition> positions, RelativeEntryModel relativeEntryModel, DriverNumberType driverNumberType)
    {
        const double borderThickness = 2.0d;
        const double localSize = 20d;
        const double otherSize = 15d;
        
        var localFill = Brushes.Red;
        var otherFill = Brushes.LightGray;
        var localBorder = Brushes.Red;
        var otherBorder = Brushes.LightGray;
            
        var number = GetDriverNumber(relativeEntryModel, driverNumberType);

        if (positions.LastOrDefault() is not { } lastPosition)
        {
            Logger?.LogError($"Failed to find last position in track map data for '{CurrentTrackName}'");
            return null;
        }

        var maxTrackLength = lastPosition.LapDistance;
        var trackDistance = maxTrackLength * relativeEntryModel.LapDistancePercentage;
        if (GetPositionOnTrack(positions, trackDistance) is not { } position)
        {
            Logger?.LogError($"Failed to calculate track map position for '{CurrentTrackName}'");
            return null;
        }
        
        var fillColor = relativeEntryModel.IsLocal ? localFill : otherFill;
        var borderColor = relativeEntryModel.IsLocal ? localBorder : otherBorder;   
        var size = relativeEntryModel.IsLocal ? localSize : otherSize;
        var halfSize = size / 2d;
        
        return new DriverTrackVisualization(position.X - halfSize,
            position.Y  - halfSize,
            size,
            number,
            fillColor,
            borderColor, 
            borderThickness);
    }

    private static int GetDriverNumber(RelativeEntryModel relativeEntryModel, DriverNumberType driverNumberType)
    {
        return driverNumberType switch
        {
            DriverNumberType.OverallPosition => relativeEntryModel.OverallPosition,
            DriverNumberType.ClassPosition => relativeEntryModel.ClassPosition,
            DriverNumberType.CarNumber => relativeEntryModel.CarNumber,
            _ => relativeEntryModel.CarNumber
        };
    }

    private static TrackMapPosition GetPositionOnTrack(List<TrackMapPosition> positions, float currentLapDistance)
    {
        TrackMapPosition positionOnTrack = null;
        
        foreach (var position in positions)
        {
            if (position.LapDistance > currentLapDistance)
                return positionOnTrack;

            positionOnTrack = new TrackMapPosition(
                position.LapDistance,
                position.X,
                position.Y,
                position.Z);
        }

        return positionOnTrack;
    }
}