namespace RacingAidWpf.Core.Logging;

public interface ILoggerFactory
{
    public ILogger GetLogger<T>();
}