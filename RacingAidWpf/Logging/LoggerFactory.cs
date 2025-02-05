namespace RacingAidWpf.Logging;

public static class LoggerFactory
{
    private static ILoggerFactory loggerFactory;

    public static ILogger GetLogger<T>()
    {
        loggerFactory ??= new Log4NetLoggerFactory();

        return loggerFactory.GetLogger<T>();
    }

    public static void SetLoggerFactory(ILoggerFactory factory)
    {
        loggerFactory = factory;
    }
}