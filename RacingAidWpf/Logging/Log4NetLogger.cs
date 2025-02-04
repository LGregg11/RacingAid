using log4net;

namespace RacingAidWpf.Logging;

public class Log4NetLogger(ILog log) : ILogger
{
    private LogLevel logLevel;

    public void LogTrace(string message)
    {
        if (logLevel >= LogLevel.Trace)
            log.Debug($"(TRACE) - {message}"); // Have to use debug for log4net
    }

    public void LogDebug(string message)
    {
        if (logLevel >= LogLevel.Debug)
            log.Debug(message);
    }

    public void LogInformation(string message)
    {
        if (logLevel >= LogLevel.Info)
            log.Info(message);
    }

    public void LogWarning(string message)
    {
        if (logLevel >= LogLevel.Info)
            log.Warn(message);
    }

    public void LogError(string message)
    {
        if (logLevel >= LogLevel.Error)
            log.Error(message);
    }

    public void LogFatal(string message)
    {
        if (logLevel >= LogLevel.Fatal)
            log.Fatal(message);
    }

    public void SetLogLevel(LogLevel level)
    {
        logLevel = level;
    }
}