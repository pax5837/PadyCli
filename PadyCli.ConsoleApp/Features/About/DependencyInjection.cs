using Microsoft.Extensions.DependencyInjection;

namespace PadyCli.ConsoleApp.Features.About;

internal static class DependencyInjection
{
    public static IServiceCollection AddAbout(this IServiceCollection services)
    {
        services.AddScoped<Implementations.About>();

        return services;
    }
}