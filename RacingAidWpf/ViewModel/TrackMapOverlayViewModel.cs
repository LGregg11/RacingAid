using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using RacingAidData.Core.Models;
using RacingAidWpf.Configuration;
using RacingAidWpf.FileHandlers;
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

    public GeometryGroup TrackMapPathData => CurrentTrackMap == null ? null : TrackMapPathCreator.Create2DGeometryGroupFromTrackMap(CurrentTrackMap, TargetSize);
    
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

    public override void Reset()
    {
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
        trackMapCreator.Stop();

        if (string.IsNullOrEmpty(CurrentTrackName))
        {
            CurrentTrackMap = null;
            return;
        }
        
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
        var driverNumberType = trackMapConfigSection.DriverNumberType;
        
        var visualizations = new ObservableCollection<DriverTrackVisualization>();
        foreach (var driver in RacingAidSingleton.Instance.Relative.Entries)
            visualizations.Add(CreateDriverTrackVisualization(scaledPositions, driver, driverNumberType));

        DriverTrackVisualizations = visualizations;
    }

    private static DriverTrackVisualization CreateDriverTrackVisualization(List<TrackMapPosition> positions, RelativeEntryModel relativeEntryModel, DriverNumberType driverNumberType)
    {
        const double borderThickness = 2.0d;
        const double localSize = 20d;
        const double otherSize = 15d;
        
        var localFill = Brushes.Red;
        var otherFill = Brushes.LightGray;
        var localBorder = Brushes.Red;
        var otherBorder = Brushes.LightGray;
            
        var number = GetDriverNumber(relativeEntryModel, driverNumberType);
        
        // TODO: Change this so that it is relative to distance and not lap percentage
        var lapsDriven = relativeEntryModel.LapsDriven;
        var lapPercentage = lapsDriven - (int)lapsDriven;
        
        var positionIndexRelativeToLapPercentage = (int)(lapPercentage * positions.Count);
        var position = positions[positionIndexRelativeToLapPercentage];
        
        var fillColor = relativeEntryModel.IsLocal ? localFill : otherFill;
        var borderColor = relativeEntryModel.IsLocal ? localBorder : otherBorder;   
        var size = relativeEntryModel.IsLocal ? localSize : otherSize;
        var halfSize = size / 2d;
        
        return new DriverTrackVisualization(position.X - halfSize,
            position.Y - halfSize,
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
}