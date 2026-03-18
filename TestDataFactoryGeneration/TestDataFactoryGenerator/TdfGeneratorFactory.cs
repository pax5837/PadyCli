using Microsoft.Extensions.DependencyInjection;

namespace TestDataFactoryGenerator;

public static class TdfGeneratorFactory
{
    public static ITestDataFactoryGenerator GetNew(
        Action<IServiceCollection> loggingRegistration,
        TdfConfigDefinition? config = null)
    {
        var services = new ServiceCollection().AddTestDataFactoryGeneration(config);
        loggingRegistration(services);

        return services.BuildServiceProvider()!.GetService<ITestDataFactoryGenerator>()!;
    }
}