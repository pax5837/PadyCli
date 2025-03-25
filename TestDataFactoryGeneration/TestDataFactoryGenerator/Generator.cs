using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace TestDataFactoryGenerator;

public static class Generator
{
    public static ITestDataFactoryGenerator GetNewGenerator()
    {
        var pathToConfig = Environment.GetEnvironmentVariable("TDF_GEN_CONFIG_FILE_PATH");
        var fileContent = File.ReadAllText(pathToConfig);

        var config = pathToConfig is not null
            ? JsonSerializer.Deserialize<TdfGeneratorConfiguration>(fileContent)
            : new TdfGeneratorConfiguration([], "    ", null, [], new SimpleTypeConfiguration(string.Empty, []));

        return GetTestDataFactoryGenerator(config!);
    }

    public static ITestDataFactoryGenerator GetNewGenerator(TdfGeneratorConfiguration tdfGeneratorConfiguration)
    {
        return GetTestDataFactoryGenerator(tdfGeneratorConfiguration);
    }

    private static ITestDataFactoryGenerator GetTestDataFactoryGenerator(TdfGeneratorConfiguration config)
    {
        var serviceProvides = new ServiceCollection()
            .AddTestDataFactoryGeneration(config)
            .BuildServiceProvider();

        return serviceProvides.GetService<ITestDataFactoryGenerator>();
    }
}