using Microsoft.Extensions.Configuration;

public static class Configurator
{
    public static IConfiguration LoadConfiguration(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        return builder.Build();
    }
}
