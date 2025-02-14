using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace RacingAidWpf.Core.Logging;

public class Log4NetLoggerFactory : ILoggerFactory
{
    private readonly Dictionary<Type, ILogger> loggers = new();

    public Log4NetLoggerFactory()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly() ?? throw new InvalidOperationException());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }
    
    public ILogger GetLogger<T>()
    {
        if (loggers.TryGetValue(typeof(T), out var logger))
            return logger;

        logger = new Log4NetLogger(LogManager.GetLogger(typeof(T)));
        loggers.Add(typeof(T), logger);

        return logger;
    }
}