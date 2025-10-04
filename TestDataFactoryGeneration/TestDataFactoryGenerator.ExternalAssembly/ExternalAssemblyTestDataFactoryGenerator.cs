using System.Collections.Immutable;
using DotnetInfrastructure.Contracts;
using Microsoft.Extensions.Logging;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

internal class ExternalAssemblyTestDataFactoryGenerator : IExternalAssemblyTestDataFactoryGenerator
{
    private readonly IAssemblyLoader _assemblyLoader;
    private readonly ITypeSelector _typeSelector;
    private readonly ITestDataFactoryGenerator _testDataFactoryGenerator;
    private readonly ILogger<ExternalAssemblyTestDataFactoryGenerator> _logger;

    public ExternalAssemblyTestDataFactoryGenerator(
        IAssemblyLoader assemblyLoader,
        ITypeSelector typeSelector,
        ITestDataFactoryGenerator testDataFactoryGenerator,
        ILogger<ExternalAssemblyTestDataFactoryGenerator> logger)
    {
        _assemblyLoader = assemblyLoader;
        _typeSelector = typeSelector;
        _testDataFactoryGenerator = testDataFactoryGenerator;
        _logger = logger;
    }

    public async Task<IImmutableList<string>> GenerateTestDataFactory(
        GenerationParameters generationParameters,
        CancellationToken cancellationToken)
    {
        var currentDirectory = generationParameters.WorkingDirectory ?? Directory.GetCurrentDirectory();

        var assembly = await _assemblyLoader.GetAssembly(
            startDirectory: currentDirectory,
            useOnTheFlyCompilation: false,
            cancellationToken: cancellationToken);

        var actualTypes = generationParameters.TypeNames
            .Select(tn => _typeSelector.SelectType(typeIdentifier: tn, assembly: assembly)
                .Switch<Type?>(
                    whenA: t => t,
                    whenB: _ => HandleNoTypeSelected(tn)))
            .OfType<Type>()
            .ToImmutableHashSet();

        return _testDataFactoryGenerator.GenerateTestDataFactory(
            testDataFactoryName: generationParameters.TestDataFactoryName,
            nameSpace: generationParameters.NameSpace,
            outputToConsole: generationParameters.OutputToConsole,
            types: actualTypes,
            includeHelperClasses: generationParameters.IncludeOptionalsCode);
    }

    private Type? HandleNoTypeSelected(string typeName)
    {
        _logger.LogInformation("No type selected for {TypeName}", typeName);
        return null;
    }
}