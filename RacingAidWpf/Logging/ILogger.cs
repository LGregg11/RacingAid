namespace RacingAidWpf.Logging;

public interface ILogger
{
    public void LogTrace(string message);
    public void LogDebug(string message);
    public void LogInformation(string message);
    public void LogWarning(string message);
    public void LogError(string message);
    public void LogFatal(string message);
    public void SetLogLevel(LogLevel level);
}