using Microsoft.Extensions.DependencyInjection;
using TestDataFactoryGenerator.Generation;
using TestDataFactoryGenerator.Generation.AbstractClasses;
using TestDataFactoryGenerator.Generation.CodeGeneration;
using TestDataFactoryGenerator.Generation.Collections;
using TestDataFactoryGenerator.Generation.Dictionnaries;
using TestDataFactoryGenerator.Generation.Either;
using TestDataFactoryGenerator.Generation.FactoryGeneration;
using TestDataFactoryGenerator.Generation.Helpers;
using TestDataFactoryGenerator.Generation.NamespaceAliases;
using TestDataFactoryGenerator.Generation.ParameterInstantiation;
using TestDataFactoryGenerator.Generation.Protobuf;
using TestDataFactoryGenerator.Generation.SimpleTypeGenerator;
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
            .AddScoped<GenerationInformation>()
            .AddScoped<IUserDefinedGenericsCodeGenerator, UserDefinedGenericsCodeGenerator>()
            .AddScoped<IEitherInformationService, EitherInformationService>()
            .AddScoped<IEitherCodeGenerator, EitherCodeGenerator>()
            .AddScoped<ITypeNameGenerator, TypeNameGenerator>()
            .AddScoped<IProtoInformationService, ProtoInformationService>()
            .AddScoped<IProtoCodeGenerator, ProtoCodeGenerator>()
            .AddScoped<ICollectionsCodeGenerator, CollectionsCodeGenerator>()
            .AddScoped<IDictionnariesCodeGenerator, DictionnariesCodeGenerator>()
            .AddScoped<IParameterInstantiationCodeGenerator, ParameterInstantiationCodeGenerator>()
            .AddScoped<ITypeWithConstructorsGenerator, TypeWithConstructorsGenerator>()
            .AddScoped<ICodeGenerator, CodeGenerator>()
            .AddScoped<ITypeLister, TypeLister>()
            .AddScoped<ITestDataFactoryGenerator, FactoryGenerator>()
            .AddScoped<ISimpleTypeGenerator, SimpleTypeGenerator>()
            .AddScoped<IAbstractClassInformationService, AbstractClassInformationService>()
            .AddScoped<IAbstractOneOfClassGenerationCreationService, AbstractOneOfClassGenerationCreationService>()
            .AddScoped<IHelpersGenerator, HelpersGenerator>()
            .AddScoped<INamespaceAliasManager, NamespaceAliasManager>();

        return services;
    }

    private static TdfGeneratorConfiguration GetConfig(TdfGeneratorConfiguration? tdfGeneratorConfiguration)
    {

        return tdfGeneratorConfiguration ?? throw new InvalidOperationException();
    }
}