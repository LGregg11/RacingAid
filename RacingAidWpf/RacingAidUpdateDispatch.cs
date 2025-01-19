using System.Diagnostics;
using RacingAidData;

namespace RacingAidWpf;

/// <summary>
/// Dispatch that triggers event updates on a timer depending on a refresh limit
/// </summary>
public static class RacingAidUpdateDispatch
{
    private static readonly RacingAid RacingAid = RacingAidSingleton.Instance;
    private static readonly SynchronizationContext? SynchronizationContext = SynchronizationContext.Current;

    private static readonly AutoResetEvent InvokeUpdateAutoResetEvent = new(false);
    private static Thread? updateThread;
    private static bool keepThreadAlive;
    private static bool modelsUpdated;

    public static event Action? Update;

    /// <remarks>
    /// If set to 0 or less the dispatch will trigger update whenever racing aid data 
    /// </remarks>
    public static int UpdateRefreshRateMs { get; set; }

    public static void Start()
    {
        if (updateThread != null || SynchronizationContext == null)
            return;

        updateThread = new Thread(UpdateLoop);
        RacingAid.ModelsUpdated += OnModelUpdated;
        
        keepThreadAlive = true;
        updateThread.Start();
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
            if (UpdateRefreshRateMs > 0)
                Thread.Sleep(UpdateRefreshRateMs);

            var invokeUpdate = modelsUpdated;
            if (!invokeUpdate)
                invokeUpdate = InvokeUpdateAutoResetEvent.WaitOne();

            if (invokeUpdate)
                InvokeUpdateOnMainThread();
            
            Trace.WriteLine("Loop end");
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