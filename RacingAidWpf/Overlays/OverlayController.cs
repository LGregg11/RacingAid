using System.IO;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Logging;
using RacingAidWpf.Resources;

namespace RacingAidWpf.Overlays;

public class OverlayController(IHandleData<OverlayPositions> overlayDataHandler, ILogger logger = null)
{
    private static readonly string OverlayPositionsJsonFullPath =
        Path.Combine(Resource.DataDirectory, "OverlayPositions.json");

    private readonly ILogger logger = logger ?? LoggerFactory.GetLogger<OverlayController>();
    private readonly List<Overlay> overlays = [];
    private bool isRepositionEnabled;

    public virtual bool IsRepositioningEnabled
    {
        get => isRepositionEnabled;
        set
        {
            if (isRepositionEnabled == value)
                return;
            
            isRepositionEnabled = value;
            foreach (var overlay in overlays)
                overlay.IsRepositionEnabled = isRepositionEnabled;
            
            if (!isRepositionEnabled)
                SaveOverlayPositions();
        }
    }

    public virtual void AddOverlay(Overlay overlay)
    {
        logger?.LogDebug($"Added '{overlay.OverlayName}' overlay");
        overlays.Add(overlay);
    }

    public void RemoveOverlay(Overlay overlay)
    {
        logger?.LogDebug($"Closing and removing '{overlay.OverlayName}' overlay");
        overlay.Close();
        overlays.Remove(overlay);
    }

    public void ShowAll()
    {
        LoadOverlayPositions();
        
        logger?.LogDebug("Showing all overlays");
        foreach (var overlay in overlays)
            overlay.Show();
    }

    public void HideAll()
    {
        logger?.LogDebug("Hiding all overlays & disabling repositioning");
        IsRepositioningEnabled = false;

        foreach (var overlay in overlays)
            overlay.Hide();
    }

    public void CloseAll()
    {
        logger?.LogDebug("Closing all overlays: repositioning disabled and clearing overlays");
        IsRepositioningEnabled = false;

        foreach (var overlay in overlays)
            overlay.Close();

        overlays.Clear();
    }

    public void ResetAll()
    {
        logger?.LogDebug("Resetting all overlays");
        foreach (var overlay in overlays)
            overlay.Reset();
    }

    private void LoadOverlayPositions()
    {
        if (!overlayDataHandler.TryDeserializeFromFile(OverlayPositionsJsonFullPath, out var overlayPositions))
        {
            logger?.LogDebug($"No overlay positions found at {OverlayPositionsJsonFullPath}");
            return;
        }
        
        logger?.LogDebug("Applying positions for overlays");
        foreach (var overlay in overlays)
        {
            if (overlayPositions.Positions.FirstOrDefault(o => o.Name == overlay.OverlayName) is not { } overlayPosition)
                continue;
            
            overlay.TopPosition = overlayPosition.Position.Top;
            overlay.LeftPosition = overlayPosition.Position.Left;
        }
    }

    private void SaveOverlayPositions()
    {
        var overlayPositions = CreateOverlayPositions();

        if (!overlayDataHandler.TrySerializeToFile(OverlayPositionsJsonFullPath, overlayPositions))
            logger?.LogError($"Failed to serialize overlay positions data to {OverlayPositionsJsonFullPath}");
        else
            logger?.LogInformation($"Saved {overlayPositions.Positions.Count} overlay positions to {OverlayPositionsJsonFullPath}");
    }

    private OverlayPositions CreateOverlayPositions()
    {
        var overlayPositionList = overlays.Select(overlay =>
                new OverlayPosition(
                    overlay.OverlayName,
                    new ScreenPosition(overlay.TopPosition, overlay.LeftPosition)))
            .ToList();

        return new OverlayPositions(overlayPositionList);
    }
        
}