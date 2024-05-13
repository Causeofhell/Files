using System;
using System.Threading;
using System.Threading.Tasks;

public class TaskScheduler
{
    private readonly TimeSpan _runTime;
    private readonly Action _taskToRun;
    private CancellationTokenSource _cancellationTokenSource;

    public TaskScheduler(TimeSpan runTime, Action taskToRun)
    {
        _runTime = runTime;
        _taskToRun = taskToRun;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start()
    {
        Task.Run(async () => await ScheduleTask(_cancellationTokenSource.Token));
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
            var nextRun = now.Date.Add(_runTime);
            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun.Subtract(now);
            if (delay > TimeSpan.Zero)
            {
                try
                {
                    await Task.Delay(delay, cancellationToken);
                    _taskToRun.Invoke();
                }
                catch (TaskCanceledException)
                {
                    return; // Task was canceled
                }
            }
        }
    }
}
