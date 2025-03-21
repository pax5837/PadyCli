using BuilderGenerator.Sources.CsFilesExplorer;
using BuilderGenerator.Sources.DataBuilders;
using Microsoft.Extensions.DependencyInjection;

namespace BuilderGenerator;

public static class ServiceConfigurator
{
    public static IServiceCollection AddDataBuilderGenerators(this IServiceCollection services)
    {
        services
            .AddScoped<IGetCandidateTypeExplorer, GetCandidateTypeExplorer>()
            .AddScoped<IDataBuilderGenerator, DataBuilderGenerator>();

        return services;
    }
}