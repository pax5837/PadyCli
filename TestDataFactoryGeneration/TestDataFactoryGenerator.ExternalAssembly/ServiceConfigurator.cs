using Microsoft.Extensions.DependencyInjection;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

public static  class ServiceConfigurator
{
    public static IServiceCollection AddExternalAssemblyTestDataFactoryGeneration(
        this IServiceCollection services,
        TdfGeneratorConfigurationOrPathToJson configurationOrPathToJson)
    {
        services
            .AddTestDataFactoryGeneration(configurationOrPathToJson)
            .AddScoped<IExternalAssemblyTestDataFactoryGenerator, ExternalAssemblyTestDataFactoryGenerator>();

        return services;
    }
}