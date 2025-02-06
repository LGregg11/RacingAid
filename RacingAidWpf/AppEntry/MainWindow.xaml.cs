using System.Windows;

namespace RacingAidWpf.AppEntry;

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
    
    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        mainWindowViewModel.Close();
        Application.Current.Shutdown();
    }
}