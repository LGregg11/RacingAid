namespace RacingAidWpf.OverlayManagement;

public interface IOverlay
{
    public string Name { get; }
    public bool IsRepositionEnabled { get; set; }
    public int TopPosition { get; set; }
    public int LeftPosition { get; set; }
    
    public void Show();
    public void Close();
}