using Files.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// Load configuration
IConfiguration configuration = Configurator.LoadConfiguration(args);

// Set up dependency injection
using (var serviceProvider = ServiceConfigurator.ConfigureService(configuration))
{
    // Read settings
    bool executeImmediately = configuration.GetValue<bool>("SchedulerSettings:ExecuteImmediately");

    // Get the file processor from the service provider
    var fileProcessor = serviceProvider.GetService<IFileProcessor>()!;

    // Create the task scheduler with the configured executeImmediately setting
    var scheduler = new TaskScheduler(() => fileProcessor.ProcessFilesAsync(), executeImmediately);

    // Start the scheduler
    scheduler.Start();

    Console.WriteLine("Task Scheduler started. Press any key to exit...");
    Console.ReadKey();

    // Stop the scheduler before exiting
    scheduler.Stop();
}
