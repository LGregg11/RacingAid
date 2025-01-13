namespace RacingAid.Core.Events;

public class DataReceivedEventArgs : EventArgs
{
    public byte[] Message { get; init; }

    public DataReceivedEventArgs(byte[] message)
    {
        Message = message;
    }
}