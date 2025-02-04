namespace RacingAidWpf.Logging;

public interface ILoggerFactory
{
    public ILogger GetLogger<T>();
}