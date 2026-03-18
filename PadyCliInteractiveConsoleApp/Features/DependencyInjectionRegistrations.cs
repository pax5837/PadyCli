using Microsoft.Extensions.DependencyInjection;
using PadyCliInteractiveConsoleApp.Features.Docker;
using PadyCliInteractiveConsoleApp.Features.GuidGeneration;
using PadyCliInteractiveConsoleApp.Features.JsonSanitizer;

namespace PadyCliInteractiveConsoleApp.Features;

internal static class DependencyInjectionRegistrations
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services
            .AddSingleton<DockerService>()
            .AddSingleton<GuidGenerator>()
            .AddSingleton<JsonSanitizerService>()
            .AddSingleton<ProcessRunner>();

        return services;
    }
}