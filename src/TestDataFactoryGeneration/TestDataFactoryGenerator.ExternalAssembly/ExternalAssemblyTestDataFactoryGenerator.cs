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

    public async Task<IImmutableList<string>> GenerateTestDataFactoryAsync(
        IExternalAssemblyTestDataFactoryGenerator.GenerationParameters generationParameters,
        CancellationToken cancellationToken)
    {
        var currentDirectory = generationParameters.ExecutionDirectory ?? Directory.GetCurrentDirectory();

        var assembly = await _assemblyLoader.GetAssemblyAsync(
            startDirectory: currentDirectory,
            useOnTheFlyCompilation: true,
            cancellationToken: cancellationToken);

        var actualTypes = generationParameters.TypeNames
            .Select(tn => _typeSelector.SelectType(tn, assembly).Switch<Type?>(t => t, _ => HandleNoTypeSelected(tn)))
            .OfType<Type>()
            .ToImmutableHashSet();

        return _testDataFactoryGenerator.GenerateTestDataFactory(
            generationParameters.TestDataFactoryName,
            generationParameters.NameSpace,
            generationParameters.OutputToConsole,
            actualTypes);
    }

    private Type? HandleNoTypeSelected(string typeName)
    {
        _logger.LogInformation("No type selected for {TypeName}", typeName);
        return null;
    }
}