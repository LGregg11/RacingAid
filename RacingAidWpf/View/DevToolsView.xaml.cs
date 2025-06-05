using System.Windows;
using RacingAidWpf.ViewModel;

namespace RacingAidWpf.View;

public partial class DevToolsView : Window
{
    private readonly DevToolsViewModel devToolViewModel = new();
    
    public DevToolsView()
    {
        InitializeComponent();
        
        DataContext = devToolViewModel;
    }
}