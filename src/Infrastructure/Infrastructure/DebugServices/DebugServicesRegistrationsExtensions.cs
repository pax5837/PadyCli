using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DebugServices;

public static class DebugServicesRegistrationsExtensions
{
    public static IServiceCollection AddDebugServices(this IServiceCollection services)
    {
        services
            .AddScoped<DebugService>()
            .AddScoped<IDebugServiceInitializer>(sp => sp.GetRequiredService<DebugService>())
            .AddScoped<IDebugService>(sp => sp.GetRequiredService<DebugService>());

        return services;
    }
}