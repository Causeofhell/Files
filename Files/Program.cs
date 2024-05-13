using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var fileProcessor = new FileProcessor();
        var scheduler = new TaskScheduler(TimeSpan.Zero, async () => await fileProcessor.ProcessFilesAsync());

        scheduler.Start(); // Start the scheduler

        Console.WriteLine("Scheduler started. Press 'Enter' to exit...");
        Console.ReadLine(); // Wait for user input to exit

        scheduler.Stop(); // Stop the scheduler
    }
}
