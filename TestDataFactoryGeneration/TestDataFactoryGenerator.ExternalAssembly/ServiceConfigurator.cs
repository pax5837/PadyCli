using Microsoft.Extensions.DependencyInjection;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

public static  class ServiceConfigurator
{
    public static IServiceCollection AddExternalAssemblyTestDataFactoryGeneration(
        this IServiceCollection services,
        TdfGeneratorConfiguration configuration)
    {
        services
            .AddTestDataFactoryGeneration(configuration)
            .AddScoped<IExternalAssemblyTestDataFactoryGenerator, ExternalAssemblyTestDataFactoryGenerator>();

        return services;
    }
}