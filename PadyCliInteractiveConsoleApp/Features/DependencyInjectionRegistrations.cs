using Microsoft.Extensions.DependencyInjection;
using PadyCliInteractiveConsoleApp.Features.Docker;
using PadyCliInteractiveConsoleApp.Features.Greetings;
using PadyCliInteractiveConsoleApp.Features.GuidGeneration;
using PadyCliInteractiveConsoleApp.Features.JsonSanitizer;
using PadyCliInteractiveConsoleApp.Features.LineFiltering;
using PadyCliInteractiveConsoleApp.Features.Logging;

namespace PadyCliInteractiveConsoleApp.Features;

internal static class DependencyInjectionRegistrations
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services
            .AddSingleton<DockerService>()
            .AddSingleton<GuidGenerator>()
            .AddSingleton<GreetingsService>()
            .AddSingleton<JsonSanitizerService>()
            .AddSingleton<LineFilteringService>()
            .AddSingleton<LogLevelChanger>()
            .AddSingleton<ProcessRunner>()
            .AddSingleton<ProtoToUmlConverter.ProtoToUmlConverter>()
            .AddSingleton<TestDataFactoryGeneration.TestDataFactoryGenerator>();

        return services;
    }
}