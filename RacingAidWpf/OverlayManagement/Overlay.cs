using System.Windows;
using System.Windows.Input;

namespace RacingAidWpf.OverlayManagement;

public class Overlay : Window
{
    public string OverlayName => Title;
    
    public bool IsRepositionEnabled { get; set; }

    public double TopPosition
    {
        get => Top;
        set => Top = value;
    }
    
    public double LeftPosition
    {
        get => Left;
        set => Left = value;
    }

    protected void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!(IsRepositionEnabled && e.LeftButton == MouseButtonState.Pressed))
            return;

        DragMove();
    }
}