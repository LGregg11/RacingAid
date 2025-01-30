using RacingAidData;
using RacingAidWpf.Configuration;

namespace RacingAidWpf;

/// <summary>
/// Dispatch that triggers event updates on a timer depending on a refresh limit
/// </summary>
public static class RacingAidUpdateDispatch
{
    private static readonly RacingAid RacingAid = RacingAidSingleton.Instance;
    private static readonly SynchronizationContext SynchronizationContext = SynchronizationContext.Current;

    private static readonly GeneralConfigSection GeneralConfigSection = ConfigSectionSingleton.GeneralSection;

    private static readonly AutoResetEvent InvokeUpdateAutoResetEvent = new(false);
    private static CancellationTokenSource updateThreadCancellationTokenSource;
    private static CancellationToken updateThreadCancellationToken;
    private static Thread updateThread;
    private static bool keepThreadAlive;
    private static bool modelsUpdated;

    /// <remarks>
    /// If set to 0 or less the dispatch will trigger update whenever racing aid data 
    /// </remarks>
    private static int updateIntervalMs = GeneralConfigSection.UpdateIntervalMs;

    public static event Action Update;

    public static void Start()
    {
        if (updateThread != null || SynchronizationContext == null)
            return;

        updateThreadCancellationTokenSource = new CancellationTokenSource();
        updateThreadCancellationToken = updateThreadCancellationTokenSource.Token;
        updateThread = new Thread(UpdateLoop);
        RacingAid.ModelsUpdated += OnModelUpdated;

        GeneralConfigSection.ConfigUpdated += OnConfigUpdated;
        
        keepThreadAlive = true;
        updateThread.Start();
    }

    private static void OnConfigUpdated()
    {
        updateIntervalMs = GeneralConfigSection.UpdateIntervalMs;
    }

    public static void Stop()
    {
        if (updateThread == null)
            return;
        
        RacingAid.ModelsUpdated -= OnModelUpdated;
        
        keepThreadAlive = false;
        
        InvokeUpdateAutoResetEvent.Set();
        updateThreadCancellationTokenSource.Cancel();
        
        updateThread?.Join();
        
        updateThread = null;
    }

    private static void UpdateLoop()
    {
        while (keepThreadAlive)
        {
            if (updateIntervalMs > 0)
                Thread.Sleep(updateIntervalMs);

            if (updateThreadCancellationToken.IsCancellationRequested)
                return;

            var shouldInvokeUpdate = modelsUpdated;
            if (!shouldInvokeUpdate)
                shouldInvokeUpdate = InvokeUpdateAutoResetEvent.WaitOne(
                    -1,
                    updateThreadCancellationToken.IsCancellationRequested);

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