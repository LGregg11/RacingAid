using System.Windows.Input;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class TimesheetWindow
{
    private readonly TimesheetWindowViewModel timesheetWindowViewModel = new();
    
    public TimesheetWindow()
    {
        InitializeComponent();
        DataContext = timesheetWindowViewModel;
    }

    private void DriversGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftCtrl))
            DragMove();
    }
}