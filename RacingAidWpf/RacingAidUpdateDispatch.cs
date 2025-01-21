using RacingAidData;
using RacingAidWpf.Configuration;

namespace RacingAidWpf;

/// <summary>
/// Dispatch that triggers event updates on a timer depending on a refresh limit
/// </summary>
public static class RacingAidUpdateDispatch
{
    private static readonly RacingAid RacingAid = RacingAidSingleton.Instance;
    private static readonly SynchronizationContext? SynchronizationContext = SynchronizationContext.Current;

    private static readonly GeneralConfigSection GeneralConfigSection = ConfigSectionSingleton.GeneralSection;

    private static readonly AutoResetEvent InvokeUpdateAutoResetEvent = new(false);
    private static Thread? updateThread;
    private static bool keepThreadAlive;
    private static bool modelsUpdated;

    /// <remarks>
    /// If set to 0 or less the dispatch will trigger update whenever racing aid data 
    /// </remarks>
    private static int updateIntervalMs = GeneralConfigSection.UpdateIntervalMs;

    public static event Action? Update;

    public static void Start()
    {
        if (updateThread != null || SynchronizationContext == null)
            return;

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
        RacingAid.ModelsUpdated -= OnModelUpdated;
        
        keepThreadAlive = false;
        InvokeUpdateAutoResetEvent.Set();
        updateThread?.Join();
        
        updateThread = null;
    }

    private static void UpdateLoop()
    {
        while (keepThreadAlive)
        {
            if (updateIntervalMs > 0)
                Thread.Sleep(updateIntervalMs);

            var invokeUpdate = modelsUpdated;
            if (!invokeUpdate)
                invokeUpdate = InvokeUpdateAutoResetEvent.WaitOne();

            if (invokeUpdate)
                InvokeUpdateOnMainThread();
        }
    }

    private static void OnModelUpdated()
    {
        modelsUpdated = true;
        InvokeUpdateAutoResetEvent.Set();
    }

    private static void InvokeUpdateOnMainThread()
    {
        InvokeUpdateAutoResetEvent.Reset();
        modelsUpdated = false;
        SynchronizationContext?.Post(_ => Update?.Invoke(), null);
    }
}