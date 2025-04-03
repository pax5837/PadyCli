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
        GenerationParameters generationParameters,
        CancellationToken cancellationToken,
        Action<string>? cliRunner = null)
    {
        var currentDirectory = generationParameters.WorkingDirectory ?? Directory.GetCurrentDirectory();

        if (cliRunner is not null)
        {
            Console.WriteLine("Do you want to run dotnet publish [y]/n? (x to exit)");
            var input = Console.ReadLine();
            if (input.ToLower() == "x")
            {
                return [];
            }

            if (string.IsNullOrEmpty(input) || input.ToLower() == "y")
            {
                cliRunner("dotnet publish --self-contained true");
            }
        }

        var assembly = await _assemblyLoader.GetAssemblyAsync(
            startDirectory: currentDirectory,
            useOnTheFlyCompilation: false,
            cancellationToken: cancellationToken);

        var actualTypes = generationParameters.TypeNames
            .Select(tn => _typeSelector.SelectType(tn, assembly).Switch<Type?>(t => t, _ => HandleNoTypeSelected(tn)))
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