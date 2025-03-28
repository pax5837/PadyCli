using Microsoft.Extensions.DependencyInjection;
using TestingHelpers.Services;
using TestingHelpers.Services.TestClassGeneration;

namespace TestingHelpers;

public static class ServiceConfigurator
{
    public static IServiceCollection AddTestHelpers(
        this IServiceCollection services,
        TestClassGeneratorConfig config)
    {
        services
            .AddSingleton(config)
            .AddSingleton<ITestClassGenerator, TestClassGenerator>()
            .AddSingleton<ITestClassCodeGenerator, TestClassCodeGenerator>();

        return services;
    }
}