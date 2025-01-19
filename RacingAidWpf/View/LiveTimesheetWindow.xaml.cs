using System.Windows.Input;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class LiveTimesheetWindow
{
    private readonly LiveTimesheetWindowViewModel liveTimesheetWindowViewModel = new();
    
    public LiveTimesheetWindow()
    {
        InitializeComponent();
        DataContext = liveTimesheetWindowViewModel;
    }

    private void DriversGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftCtrl))
            DragMove();
    }
}