using System.Windows;

namespace RacingAidWpf.OverlayManagement;

public class Overlay : Window, IOverlay
{
    public bool IsRepositionEnabled { get; set; }
    public int TopPosition { get; set; }
    public int LeftPosition { get; set; }
}