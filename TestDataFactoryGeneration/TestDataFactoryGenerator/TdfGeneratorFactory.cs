using Microsoft.Extensions.DependencyInjection;

namespace TestDataFactoryGenerator;

public static class TdfGeneratorFactory
{
    public static ITestDataFactoryGenerator GetNew(TdfConfigDefinition? config = null)
    {
        var serviceProvides = new ServiceCollection()
            .AddTestDataFactoryGeneration(config)
            .BuildServiceProvider();

        return serviceProvides.GetService<ITestDataFactoryGenerator>();
    }
}