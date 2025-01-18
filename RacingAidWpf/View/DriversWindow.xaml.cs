using System.Windows.Input;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class DriversWindow
{
    private readonly DriversWindowViewModel driversWindowViewModel = new();
    
    public DriversWindow()
    {
        InitializeComponent();
        DataContext = driversWindowViewModel;
    }

    private void DriversGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftCtrl))
            DragMove();
    }
}