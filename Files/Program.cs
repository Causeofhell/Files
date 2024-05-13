
using Files.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Build configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        IConfiguration configuration = builder.Build();

        // Set up dependency injection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection, configuration);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Example of using a service
        var fileProcessor = serviceProvider.GetService<IFileProcessor>();
        fileProcessor.ProcessFilesAsync().Wait();
    }

    static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfiguration>(configuration);

        // Register Logger as ILog
        services.AddSingleton<ILog, Logger>();
        services.AddSingleton<IMetadataExtractor, MetadataExtractor>();



        services.AddSingleton<IFileProcessor, FileProcessor>((serviceProvider) =>
        {
            var logger = serviceProvider.GetRequiredService<ILog>();
            var metadataExtractor = serviceProvider.GetRequiredService<IMetadataExtractor>();  // This line assumes you have registered IMetadataExtractor as shown above
            var directoryPath = configuration.GetValue<string>("FileProcessorSettings:DirectoryPath");
            var fileExtension = configuration.GetValue<string>("FileProcessorSettings:FileExtension");
            return new FileProcessor(logger, metadataExtractor, directoryPath, fileExtension);
        });

        // Add other services and configurations
    }
}
