using System.IO;
using Newtonsoft.Json;
using RacingAidWpf.Resources;

namespace RacingAidWpf.OverlayManagement;

public class OverlayController
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
        if (!File.Exists(OverlayPositionsJsonFullPath))
            return;

        var overlayPositionsJsonString = File.ReadAllText(OverlayPositionsJsonFullPath);
        
        if (JsonConvert.DeserializeObject<List<OverlayPosition>>(overlayPositionsJsonString) is not { } overlayPositions)
            return;

        foreach (var overlay in overlays)
        {
            if (overlayPositions.FirstOrDefault(o => o.Name == overlay.OverlayName) is not { } overlayPosition)
                continue;
            
            overlay.TopPosition = overlayPosition.Position.Top;
            overlay.LeftPosition = overlayPosition.Position.Left;
        }
    }

    private void SaveOverlayPositions()
    {
        var overlayPositions = overlays.Select(CreateOverlayPosition).ToList();

        if (JsonConvert.SerializeObject(overlayPositions, Formatting.Indented) is { } overlayPositionsJsonString)
            File.WriteAllTextAsync(OverlayPositionsJsonFullPath, overlayPositionsJsonString);
    }

    private static OverlayPosition CreateOverlayPosition(Overlay overlay) =>
        new(overlay.OverlayName, new ScreenPosition(overlay.TopPosition, overlay.LeftPosition));
}