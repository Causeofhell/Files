using Files.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

public class Logger : ILog
{
    private readonly string _logFilePath;

    public Logger(IConfiguration configuration)
    {
        // Read the directory path from the configuration
        string directoryPath = configuration.GetValue<string>("FileProcessorSettings:DirectoryPath");
        _logFilePath = Path.Combine(directoryPath, "log.txt");

        // Ensure the directory exists
        Directory.CreateDirectory(directoryPath);
    }

    public void Log(string message)
    {
        // Build the complete message with date and time.
        string logMessage = $"{DateTime.Now}: {message}";

        // Write the message to the console.
        Console.WriteLine(logMessage);

        // Ensure that the log file is written securely even in multi-threaded environments.
        lock (_logFilePath)
        {
            // Write the message to 'log.txt'.
            using (StreamWriter sw = new StreamWriter(_logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }
        }
    }
}
