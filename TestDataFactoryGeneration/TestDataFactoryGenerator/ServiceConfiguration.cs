using Microsoft.Extensions.DependencyInjection;
using TestDataFactoryGenerator.Generation;
using TestDataFactoryGenerator.Generation.Collections;
using TestDataFactoryGenerator.Generation.Either;
using TestDataFactoryGenerator.Generation.Optionals;
using TestDataFactoryGenerator.Generation.ParameterInstantiation;
using TestDataFactoryGenerator.Generation.Protobuf;
using TestDataFactoryGenerator.Generation.TypeNames;
using TestDataFactoryGenerator.Generation.TypesWithConstructors;
using TestDataFactoryGenerator.Generation.UserDefinedGenerics;

namespace TestDataFactoryGenerator;

public static class ServiceConfiguration
{
    public static IServiceCollection AddTestDataFactoryGeneration(
        this IServiceCollection services,
        TdfConfigDefinition? tdfGeneratorConfigurationOrPathToJson)
    {
        var config = ConfigProvider.GetConfiguration(tdfGeneratorConfigurationOrPathToJson);

        services
            .AddSingleton<TdfGeneratorConfiguration>(config)
            .AddScoped<IUserDefinedGenericsCodeGenerator, UserDefinedGenericsCodeGenerator>()
            .AddScoped<IEitherInformationService, EitherInformationService>()
            .AddScoped<IEitherCodeGenerator, EitherCodeGenerator>()
            .AddScoped<ITypeNameGenerator, TypeNameGenerator>()
            .AddScoped<IProtoInformationService, ProtoInformationService>()
            .AddScoped<IProtoCodeGenerator, ProtoCodeGenerator>()
            .AddScoped<ICollectionsCodeGenerator, CollectionsCodeGenerator>()
            .AddScoped<IParameterInstantiationCodeGenerator, ParameterInstantiationCodeGenerator>()
            .AddScoped<ITypeWithConstructorsGenerator, TypeWithConstructorsGenerator>()
            .AddScoped<ICodeGenerator, CodeGenerator>()
            .AddScoped<ITypeLister, TypeLister>()
            .AddScoped<ITestDataFactoryGenerator, FactoryGenerator>()
            .AddScoped<IRandomizerCallerGenerator, RandomizerCallerGenerator>()
            .AddScoped<IOptionalsGenerator, OptionalsGenerator>();

        return services;
    }

    private static TdfGeneratorConfiguration GetConfig(TdfGeneratorConfiguration? tdfGeneratorConfiguration)
    {

        return tdfGeneratorConfiguration ?? throw new InvalidOperationException();
    }
}