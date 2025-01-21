using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;
using RacingAidWpf.Resources;

namespace RacingAidWpf.OverlayManagement;

public class OverlayPositionController
{
    private static readonly string OverlayPositionJsonPath =
        Path.Combine(Resource.ExecutingAssembly.Location, "Overlays", "OverlayPositions.json");
    
    private readonly List<IOverlay> overlays = [];
    private bool isRepositionEnabled;

    public bool IsRepositioningEnabled
    {
        get => isRepositionEnabled;
        set
        {
            isRepositionEnabled = value;
            
            foreach (var overlay in overlays)
                overlay.IsRepositionEnabled = isRepositionEnabled;
        }
    }

    public void AddOverlay(IOverlay overlay)
    {
        overlays.Add(overlay);
    }

    public void RemoveOverlay(IOverlay overlay)
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

    public void CloseAll()
    {
        SaveOverlayPositions();
        
        foreach(var overlay in overlays)
            overlay.Close();
    }

    private void LoadOverlayPositions()
    {
        if (JsonConvert.DeserializeObject<List<OverlayPosition>>(OverlayPositionJsonPath) is not { } overlayPositions)
            return;

        foreach (var overlay in overlays)
        {
            if (overlayPositions.FirstOrDefault(o => o.Name == overlay.Name) is not { } overlayPosition)
                continue;
            
            overlay.TopPosition = overlayPosition.Position.Top;
            overlay.LeftPosition = overlayPosition.Position.Left;
        }
    }

    private void SaveOverlayPositions()
    {
        var overlayPositions = overlays.Select(CreateOverlayPosition).ToList();

        if (JsonConvert.SerializeObject(overlayPositions) is { } overlayPositionsJsonString)
            File.WriteAllTextAsync(OverlayPositionJsonPath, overlayPositionsJsonString);
    }

    private static OverlayPosition CreateOverlayPosition(IOverlay overlay) =>
        new(overlay.Name, new Position(overlay.TopPosition, overlay.LeftPosition));
}