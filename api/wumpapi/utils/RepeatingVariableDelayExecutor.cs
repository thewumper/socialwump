namespace wumpapi.utils;

public class RepeatingVariableDelayExecutor(
    RepeatingVariableDelayExecutor.DelayDelegate delayDelegate,
    TimeSpan initialDelay,
    ILogger logger)
{
    readonly DelayDelegate delayDelegate = delayDelegate;
    TimeSpan delay = initialDelay;
    readonly ILogger logger = logger;
    private CancellationTokenSource cts = new();
    public void Start()
    {
        _ = RunRecursive(cts.Token);
    }

    private async Task RunRecursive(CancellationToken token )
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(delay, cts.Token);
                delay = delayDelegate();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
            }
        }
    }
    

    public void Stop()
    {
        cts.Cancel();
        cts.Dispose();
        cts = new CancellationTokenSource();
    }
    
    public delegate TimeSpan DelayDelegate();
}