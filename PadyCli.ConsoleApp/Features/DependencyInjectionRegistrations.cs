using Microsoft.Extensions.DependencyInjection;
using PadyCli.ConsoleApp.Features.CsProjectMover;
using PadyCli.ConsoleApp.Features.Docker;
using PadyCli.ConsoleApp.Features.GuidGeneration;
using PadyCli.ConsoleApp.Features.ProtoToUmlConverter;
using PadyCli.ConsoleApp.Features.TestClassGeneration;

namespace PadyCli.ConsoleApp.Features;

internal static class DependencyInjectionRegistrations
{
    public static IServiceCollection AddFeatures(this IServiceCollection services)
    {
        services
            .AddSingleton<ProtToUmlConverter>()
            .AddSingleton<TestClassGeneratorAdapter>()
            .AddSingleton<GuidGenerator>()
            .AddSingleton<CsProjectMoverAdapter>()
            .AddSingleton<ProcessRunner>()
            .AddSingleton<DockerService>()
            .AddSingleton<TestDataFactoryGeneration.TestDataFactoryGenerator>();

        return services;
    }
}