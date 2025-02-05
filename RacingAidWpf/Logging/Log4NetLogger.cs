using log4net;

namespace RacingAidWpf.Logging;

public class Log4NetLogger : ILogger
{
    private readonly ILog logger;
    private LogLevel logLevel;

    public Log4NetLogger(ILog log)
    {
        logger = log;
        logLevel = GetLogLevel(log);
    }

    public void LogTrace(string message)
    {
        if (logLevel >= LogLevel.Trace)
            logger.Debug($"(TRACE) - {message}"); // Have to use debug for log4net
    }

    public void LogDebug(string message)
    {
        if (logLevel >= LogLevel.Debug)
            logger.Debug(message);
    }

    public void LogInformation(string message)
    {
        if (logLevel >= LogLevel.Info)
            logger.Info(message);
    }

    public void LogWarning(string message)
    {
        if (logLevel >= LogLevel.Warn)
            logger.Warn(message);
    }

    public void LogError(string message)
    {
        if (logLevel >= LogLevel.Error)
            logger.Error(message);
    }

    public void LogFatal(string message)
    {
        if (logLevel >= LogLevel.Fatal)
            logger.Fatal(message);
    }

    public void SetLogLevel(LogLevel level)
    {
        logLevel = level;
    }

    private static LogLevel GetLogLevel(ILog log)
    {
        if (log.IsDebugEnabled)
            return LogLevel.Debug;
        if (log.IsInfoEnabled)
            return LogLevel.Info;
        if (log.IsWarnEnabled)
            return LogLevel.Warn;
        if (log.IsErrorEnabled)
            return LogLevel.Error;
        if (log.IsFatalEnabled)
            return LogLevel.Fatal;

        return LogLevel.Trace;
    }
}