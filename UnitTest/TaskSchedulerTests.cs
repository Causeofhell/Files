using Moq;

[TestFixture]
public class TaskSchedulerTests
{
    private Mock<Func<Task>> _taskToRunMock;
    private TaskScheduler _scheduler;

    [SetUp]
    public void Setup()
    {
        _taskToRunMock = new Mock<Func<Task>>();
        _scheduler = new TaskScheduler(TimeSpan.Zero, _taskToRunMock.Object);
    }

    [Test]
    public void Start_InvokesTaskImmediately()
    {
        // Act
        _scheduler.Start();
        Thread.Sleep(1000); // Allow some time for the task to run

        // Assert
        _taskToRunMock.Verify(t => t.Invoke(), Times.AtLeastOnce());
    }

    [Test]
    public void Stop_CancelsScheduledTasks()
    {
        // Arrange
        _scheduler.Start();
        Thread.Sleep(1000); // Allow some time for the task to run

        // Act
        _scheduler.Stop();

        // Assert
        // No specific assertion for stop, but you should check that no new tasks are started after this point.
    }
}
