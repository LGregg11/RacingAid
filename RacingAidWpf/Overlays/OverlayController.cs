using System.IO;
using RacingAidWpf.FileHandlers;
using RacingAidWpf.Resources;

namespace RacingAidWpf.Overlays;

public class OverlayController(IHandleData<OverlayPositions> dataHandler)
{
    private static readonly string OverlayPositionsJsonFullPath =
        Path.Combine(Resource.DataDirectory, "OverlayPositions.json");
    
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
        overlays.Add(overlay);
    }

    public void RemoveOverlay(Overlay overlay)
    {
        overlay.Close();
        overlays.Remove(overlay);
    }

    public void ShowAll()
    {
        LoadOverlayPositions();
        
        foreach (var overlay in overlays)
            overlay.Show();
    }

    public void HideAll()
    {
        IsRepositioningEnabled = false;

        foreach (var overlay in overlays)
            overlay.Hide();
    }

    public void CloseAll()
    {
        IsRepositioningEnabled = false;

        foreach (var overlay in overlays)
            overlay.Close();

        overlays.Clear();
    }

    private void LoadOverlayPositions()
    {
        if (!dataHandler.TryDeserializeFromFile(OverlayPositionsJsonFullPath, out var overlayPositions))
            return;

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

        if (!dataHandler.TrySerializeToFile(OverlayPositionsJsonFullPath, overlayPositions))
        {
            // TODO: Add logging here
        }
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