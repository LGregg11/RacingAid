using System.Windows;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public sealed partial class MainWindow
{
    private readonly MainWindowViewModel mainWindowViewModel = new();
    
    public MainWindow()
    {
        InitializeComponent();
        
        DataContext = mainWindowViewModel;
    }


    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        StopButton.Command.Execute(sender);
        Application.Current.Shutdown();
    }
}