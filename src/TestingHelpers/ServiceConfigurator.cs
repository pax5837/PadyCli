using Microsoft.Extensions.DependencyInjection;
using TestingHelpers.Services;
using TestingHelpers.Services.TestClassGeneration;

namespace TestingHelpers;

public static class ServiceConfigurator
{
    public static IServiceCollection AddTestHelpers(this IServiceCollection services)
    {
        services
            .AddSingleton<ITestClassGenerator, TestClassGenerator>()
            .AddSingleton<ITestClassCodeGenerator, TestClassCodeGenerator>();

        return services;
    }
}