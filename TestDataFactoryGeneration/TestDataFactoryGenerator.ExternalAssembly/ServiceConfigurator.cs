using Microsoft.Extensions.DependencyInjection;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

public static class ServiceConfigurator
{
    public static IServiceCollection AddExternalAssemblyTestDataFactoryGeneration(
        this IServiceCollection services,
        TdfConfigDefinition configDefinition)
    {
        services
            .AddTestDataFactoryGeneration(configDefinition)
            .AddScoped<IExternalAssemblyTestDataFactoryGenerator, ExternalAssemblyTestDataFactoryGenerator>();

        return services;
    }
}