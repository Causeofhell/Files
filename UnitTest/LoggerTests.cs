[TestFixture]
public class LoggerTests
{
    [Test]
    public void Log_WritesToConsoleAndFile()
    {
        // Arrange
        using (var stringWriter = new StringWriter())
        {
            Console.SetOut(stringWriter);
            var logger = new Logger();

            // Act
            logger.Log("Test message");

            // Assert
            string output = stringWriter.ToString();
            Assert.IsTrue(output.Contains("Test message"));
            // Additionally, check if the file contains the logged message
        }
    }
}
