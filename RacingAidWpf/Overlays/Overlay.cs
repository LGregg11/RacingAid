using System.Windows;
using System.Windows.Input;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.Overlays;

public class Overlay : Window
{
    private OverlayViewModel ViewModel => DataContext as OverlayViewModel; 
    
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

    public void Reset()
    {
        ViewModel.Reset();
    }

    protected void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!(IsRepositionEnabled && e.LeftButton == MouseButtonState.Pressed))
            return;

        DragMove();
    }
}