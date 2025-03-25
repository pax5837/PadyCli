using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TestDataFactoryGenerator.Generation;

namespace TestDataFactoryGenerator;

public static class Generator
{
    public static ITestDataFactoryGenerator GetNewGenerator()
    {
        var pathToConfig = Environment.GetEnvironmentVariable("TDF_GEN_CONFIG_FILE_PATH");
        var fileContent = File.ReadAllText(pathToConfig);

        var config = pathToConfig is not null
            ? JsonSerializer.Deserialize<Configuration>(fileContent)
            : new Configuration([], null, []);

        return GetTestDataFactoryGenerator(config!);
    }

    public static ITestDataFactoryGenerator GetNewGenerator(IConfiguration configuration)
    {
        return GetTestDataFactoryGenerator(configuration);
    }

    private static ITestDataFactoryGenerator GetTestDataFactoryGenerator(IConfiguration config)
    {
        var serviceProvides = new ServiceCollection()
            .AddTestDataFactoryGeneration(config)
            .BuildServiceProvider();

        return serviceProvides.GetService<ITestDataFactoryGenerator>();
    }
}