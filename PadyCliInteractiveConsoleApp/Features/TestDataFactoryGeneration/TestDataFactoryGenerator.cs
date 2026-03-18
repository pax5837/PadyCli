using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Spectre.Console;
using TestDataFactoryGenerator.TypeSelectionWrapper;
using TextCopy;

namespace PadyCliInteractiveConsoleApp.Features.TestDataFactoryGeneration;

internal class TestDataFactoryGenerator
{
    private readonly IExternalAssemblyTestDataFactoryGenerator _externalAssemblyTestDataFactoryGenerator;
    private readonly ILogger<TestDataFactoryGenerator> _logger;

    public TestDataFactoryGenerator(
        IExternalAssemblyTestDataFactoryGenerator externalAssemblyTestDataFactoryGenerator,
        ILogger<TestDataFactoryGenerator> logger)
    {
        _externalAssemblyTestDataFactoryGenerator = externalAssemblyTestDataFactoryGenerator;
        _logger = logger;
    }

    public async Task<OperationResult> RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var directory = AnsiConsole.Ask<string>("Enter folder or confirm current", currentDirectory);
        
            var tdfName = AnsiConsole.Ask<string>("Enter the TestDataFactory class name");
            var tdfNameSpace = AnsiConsole.Ask<string>("Enter the TestDataFactory namespace") ?? string.Empty;
        
            var topLevelClassNamesToInclude = AnsiConsole.Ask<string>("Enter the names of the classes to include in the generator (comma ',' separated)");
        
        
            var generationParameters = new GenerationParameters(
                TestDataFactoryName: tdfName,
                NameSpace: tdfNameSpace,
                TypeNames: topLevelClassNamesToInclude.Split(',').ToImmutableHashSet(),
                OutputToConsole: true,
                WorkingDirectory: directory,
                IncludeOptionalsCode: false);

            var lines = await _externalAssemblyTestDataFactoryGenerator
                .GenerateTestDataFactory(generationParameters, cancellationToken);

            ClipboardService.SetText(string.Join("\n", lines));

            return OperationResult.Continue;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while generating TestDataFactory, see inner exception for details");
            throw;
        }
    }
}