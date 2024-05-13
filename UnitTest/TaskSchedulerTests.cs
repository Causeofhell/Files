using Moq;

[TestFixture]
public class TaskSchedulerTests
{
    private Mock<Func<Task<bool>>> _taskMock;
    private TaskScheduler _scheduler;

    [SetUp]
    public void Setup()
    {
        _taskMock = new Mock<Func<Task<bool>>>();
        // Setup the task mock to by default return a completed task with 'true' indicating more work to do
        _taskMock.Setup(m => m()).ReturnsAsync(true);
    }

    [Test]
    public async Task Start_ExecutesImmediately_ExecutesTaskImmediately()
    {
        // Arrange
        _scheduler = new TaskScheduler(_taskMock.Object, executeImmediately: true);

        // Act
        var startTask = Task.Run(() => _scheduler.Start());
        await Task.Delay(100); // Give some time for the task to start and execute
        _scheduler.Stop();
        await startTask;

        // Assert
        _taskMock.Verify(m => m(), Times.AtLeastOnce());
    }

    [Test]
    public async Task Start_ExecutesAtMidnight_WaitsUntilMidnight()
    {
        // Arrange
        _scheduler = new TaskScheduler(_taskMock.Object, executeImmediately: false);
        _taskMock.Setup(m => m()).ReturnsAsync(false); // No more work after one run

        // Act
        var startTask = Task.Run(() => _scheduler.Start());
        await Task.Delay(10); // Give some time for the task to start
        _scheduler.Stop();
        await startTask;

        // Assert
        _taskMock.Verify(m => m(), Times.Never()); // Ensure the task has not been executed
    }

    [Test]
    public async Task Stop_StopsScheduler_NoFurtherTasksExecuted()
    {
        // Arrange
        _scheduler = new TaskScheduler(_taskMock.Object, executeImmediately: true);

        // Act
        var startTask = Task.Run(() => _scheduler.Start());
        await Task.Delay(100); // Allow the task to start and run
        _scheduler.Stop();
        await startTask; // Ensure it completes after stopping

        // Let's trigger another check to ensure no tasks run after stopping
        await Task.Delay(2000);

        // Assert
        _taskMock.Verify(m => m(), Times.AtMostOnce()); // Should only have been called once, not more after stop
    }
}
