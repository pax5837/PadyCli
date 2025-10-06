using DotnetInfrastructure.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetInfrastructure;

public static class ServiceConfigurator
{
    public static IServiceCollection AddDotnetInfrastructure(this IServiceCollection services)
    {
        services
            .AddSingleton<IAssemblyLoader, AssemblyLoader>()
            .AddSingleton<AssemblyInfo>()
            .AddSingleton<ITypeSelector, TypeSelector>();

        return services;
    }
}