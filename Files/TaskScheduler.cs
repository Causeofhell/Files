public class TaskScheduler
{
    private readonly TimeSpan _runTime;
    private readonly Func<Task> _taskToRunAsync;  // Changed to Func<Task>
    private CancellationTokenSource _cancellationTokenSource;

    public TaskScheduler(TimeSpan runTime, Func<Task> taskToRunAsync)
    {
        _runTime = runTime;
        _taskToRunAsync = taskToRunAsync;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            // Execute the task immediately for debugging
            await _taskToRunAsync();  // Now awaiting the task
            await ScheduleTask(_cancellationTokenSource.Token);
        });
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task ScheduleTask(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var todayMidnight = now.Date.AddDays(1); // Midnight of the next day
            var delay = todayMidnight - now;

            try
            {
                await Task.Delay(delay, cancellationToken);
                await _taskToRunAsync();  // Now awaiting the task
            }
            catch (TaskCanceledException)
            {
                return;  // Task was canceled
            }
        }
    }
}
