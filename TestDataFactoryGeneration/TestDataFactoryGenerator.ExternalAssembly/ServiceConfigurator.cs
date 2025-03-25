using Microsoft.Extensions.DependencyInjection;
using TestDataFactoryGenerator.Generation;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

public static  class ServiceConfigurator
{
    public static IServiceCollection AddExternalAssemblyTestDataFactoryGeneration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddTestDataFactoryGeneration(configuration)
            .AddScoped<IExternalAssemblyTestDataFactoryGenerator, ExternalAssemblyTestDataFactoryGenerator>();

        return services;
    }
}