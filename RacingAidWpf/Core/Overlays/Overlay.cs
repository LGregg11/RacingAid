using System.Windows;
using System.Windows.Input;

namespace RacingAidWpf.Core.Overlays;

public class Overlay : Window
{
    private OverlayViewModel ViewModel => DataContext as OverlayViewModel; 
    
    public string OverlayName => Title;

    public event Action<Overlay, bool> IsOverlayEnabledToggled;


    private bool isOverlayEnabled = true;

    public bool IsOverlayEnabled
    {
        get => isOverlayEnabled;
        set
        {
            if (isOverlayEnabled == value)
                return;
            
            isOverlayEnabled = value;
            IsOverlayEnabledToggled?.Invoke(this, isOverlayEnabled);
        }
    }
    
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