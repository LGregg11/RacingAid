using RacingAidData;
using RacingAidWpf.Core.Configuration;
using RacingAidWpf.Core.Singleton;

namespace RacingAidWpf.Core.Dispatchers;

/// <summary>
/// Dispatch that triggers event updates on a timer depending on a refresh limit
/// </summary>
public static class RacingAidUpdateDispatch
{
    private static readonly RacingAid RacingAid = RacingAidSingleton.Instance;
    private static readonly SynchronizationContext SynchronizationContext = SynchronizationContext.Current;

    private static readonly GeneralConfigSection GeneralConfigSection = ConfigSectionSingleton.GeneralSection;

    private static readonly AutoResetEvent InvokeUpdateAutoResetEvent = new(false);
    private static CancellationTokenSource cancellationTokenSource;
    private static CancellationToken cancellationToken;
    private static Thread updateLoopThread;
    private static bool modelsUpdated;

    /// <remarks>
    /// If set to 0 or less the dispatch will trigger update whenever racing aid data 
    /// </remarks>
    private static int updateIntervalMs = GeneralConfigSection.UpdateIntervalMs;

    public static event Action Update;

    public static void Start()
    {
        if (updateLoopThread != null || SynchronizationContext == null)
            return;
        
        RacingAid.ModelsUpdated += OnModelUpdated;
        GeneralConfigSection.ConfigUpdated += OnConfigUpdated;
        
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;
        updateLoopThread = new Thread(UpdateLoop);
        updateLoopThread.Start();
    }

    private static void OnConfigUpdated()
    {
        updateIntervalMs = GeneralConfigSection.UpdateIntervalMs;
    }

    public static void Stop()
    {
        if (updateLoopThread == null)
            return;
        
        RacingAid.ModelsUpdated -= OnModelUpdated;
        
        cancellationTokenSource.Cancel();

        InvokeUpdateAutoResetEvent.Set();

        updateLoopThread.Join();
        updateLoopThread = null;
    }

    private static void UpdateLoop()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (updateIntervalMs > 0)
                Thread.Sleep(updateIntervalMs);

            if (cancellationToken.IsCancellationRequested)
                break;

            var shouldInvokeUpdate = modelsUpdated;
            if (!shouldInvokeUpdate)
            {
                // Wait for the event to be triggered - unless the cancellation token is requested
                shouldInvokeUpdate =
                    InvokeUpdateAutoResetEvent.WaitOne(-1) && !cancellationToken.IsCancellationRequested;
            }

            if (shouldInvokeUpdate)
                InvokeUpdate();
        }
    }

    private static void OnModelUpdated()
    {
        modelsUpdated = true;
        InvokeUpdateAutoResetEvent.Set();
    }

    private static void InvokeUpdate()
    {
        InvokeUpdateAutoResetEvent.Reset();
        modelsUpdated = false;
        Update?.Invoke();
    }
}