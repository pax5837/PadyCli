using System.Collections.Immutable;
using DotnetInfrastructure.Contracts;
using Microsoft.Extensions.Logging;
using TestingHelpers.Services.TestClassGeneration;

namespace TestingHelpers.Services;

internal class TestClassGenerator : ITestClassGenerator
{
    private readonly IAssemblyLoader _assemblyLoader;
    private readonly ITypeSelector _typeSelector;
    private readonly ITestClassCodeGenerator _codeGenerator;
    private readonly ILogger<TestClassGenerator> _logger;

    public TestClassGenerator(
        IAssemblyLoader assemblyLoader,
        ITypeSelector typeSelector,
        ITestClassCodeGenerator codeGenerator,
        ILogger<TestClassGenerator> logger)
    {
        this._assemblyLoader = assemblyLoader;
        this._typeSelector = typeSelector;
        this._codeGenerator = codeGenerator;
        this._logger = logger;
    }

    public async Task<IImmutableList<string>> GenerateTestClassAsync(
        string targetClassName,
        bool compileOnTheFly,
        CancellationToken cancellationToken)
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var assembly = await _assemblyLoader.GetAssemblyAsync(
            startDirectory: currentDirectory,
            useOnTheFlyCompilation: compileOnTheFly,
            cancellationToken: cancellationToken);
        var typeToProcessOrExit = _typeSelector.SelectType(targetClassName, assembly);

        return typeToProcessOrExit.Switch(
            whenA: ProcessType,
            whenB: _ => []);
    }

    private IImmutableList<string> ProcessType(Type type)
    {
        _logger.LogInformation($"Processing type: {type.FullName}");

        return _codeGenerator.GenerateTestClass(type);
    }
}