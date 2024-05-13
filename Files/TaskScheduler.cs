public class TaskScheduler
{
    private readonly Func<Task<bool>> _taskToRunAsync;  // Assume Task returns bool indicating whether to continue
    private CancellationTokenSource _cancellationTokenSource;

    public bool ExecuteImmediately { get; set; }

    public TaskScheduler(Func<Task<bool>> taskToRunAsync, bool executeImmediately = false)
    {
        _taskToRunAsync = taskToRunAsync;
        _cancellationTokenSource = new CancellationTokenSource();
        ExecuteImmediately = executeImmediately;
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            if (ExecuteImmediately)
            {
                // Execute the task immediately
                bool continueRunning = await _taskToRunAsync();
                if (!continueRunning) return;  // Stop if no more files to process
            }

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                bool continueRunning = await ScheduleTask(_cancellationTokenSource.Token);
                if (!continueRunning) break;  // Break the loop if no more files to process
            }
        });
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task<bool> ScheduleTask(CancellationToken cancellationToken)
    {
        try
        {
            var delay = CalculateDelayUntilMidnight();
            await Task.Delay(delay, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                return await _taskToRunAsync();
            }
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;  // Task was canceled, possibly by calling Stop
        }
    }

    private TimeSpan CalculateDelayUntilMidnight()
    {
        var now = DateTime.Now;
        var midnightToday = now.Date.AddDays(1);
        return midnightToday - now;
    }
}
