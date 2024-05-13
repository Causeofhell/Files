using Files.Interface;
using Files.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceConfigurator
{
    public static ServiceProvider ConfigureService(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        return services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfiguration>(configuration);

        // Register Logger as ILog with dependency on IConfiguration
        services.AddSingleton<ILog, Logger>();
        services.AddSingleton<IMetadataExtractor, MetadataExtractor>();
        services.AddSingleton<IFileService, FileService>();

        services.AddSingleton<IFileProcessor, FileProcessor>((serviceProvider) =>
        {
            var logger = serviceProvider.GetRequiredService<ILog>();
            var metadataExtractor = serviceProvider.GetRequiredService<IMetadataExtractor>();
            var fileService = serviceProvider.GetRequiredService<IFileService>();
            var directoryPath = configuration.GetValue<string>("FileProcessorSettings:DirectoryPath");
            var fileExtension = configuration.GetValue<string>("FileProcessorSettings:FileExtension");
            return new FileProcessor(logger, metadataExtractor, fileService, directoryPath, fileExtension);
        });
    }
}
