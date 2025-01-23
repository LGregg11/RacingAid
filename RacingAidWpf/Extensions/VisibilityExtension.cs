using System.Windows;

namespace RacingAidWpf.Extensions;

public static class VisibilityExtensions
{
    public static Visibility ToVisibility(this bool isVisible)
    {
        return isVisible ? Visibility.Visible : Visibility.Hidden;
    }
}