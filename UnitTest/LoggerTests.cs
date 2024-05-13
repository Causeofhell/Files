using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

[TestFixture]
public class LoggerTests
{
    private Logger _logger;
    private string _testDirectory;
    private string _logFilePath;
    private Mock<IConfiguration> _configurationMock;
    private Mock<IConfigurationSection> _configurationSectionMock;

    [SetUp]
    public void SetUp()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _logFilePath = Path.Combine(_testDirectory, "log.txt");

        // Create a unique directory for each test to avoid conflicts
        Directory.CreateDirectory(_testDirectory);

        // Mock the IConfigurationSection
        _configurationSectionMock = new Mock<IConfigurationSection>();
        _configurationSectionMock.SetupGet(x => x.Value).Returns(_testDirectory);

        // Mock the IConfiguration
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c.GetSection("FileProcessorSettings:DirectoryPath")).Returns(_configurationSectionMock.Object);

        // Initialize the Logger with the mocked IConfiguration
        _logger = new Logger(_configurationMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, true);
    }

    [Test]
    public void Log_WritesMessageToFile()
    {
        // Arrange
        string testMessage = "Test log message";

        // Act
        _logger.Log(testMessage);

        // Assert
        Assert.IsTrue(File.Exists(_logFilePath), "Log file should exist.");
        string loggedText = File.ReadAllText(_logFilePath);
        StringAssert.Contains(testMessage, loggedText, "The log file should contain the test message.");
        StringAssert.Contains(DateTime.Now.Year.ToString(), loggedText, "The log should contain the current year.");
    }
}
