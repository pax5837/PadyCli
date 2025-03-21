using CsProjMover.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace CsProjMover;

public static class ServiceConfigurator
{
    public static IServiceCollection AddProjectMoverServices(this IServiceCollection services)
    {
        services
            .AddScoped<IProjectMoverService, ProjectMoverService>()
            .AddScoped<ISolutionFileUpdateService, SolutionFileUpdateService>()
            .AddScoped<IProjectFileUpdateService, ProjectFileUpdateService>()
            .AddScoped<IOptionsValidationService, OptionsValidationService>();

        return services;
    }
}