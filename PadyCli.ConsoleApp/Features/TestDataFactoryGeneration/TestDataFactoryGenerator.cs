using System.Collections.Immutable;
using Serilog.Events;
using TestDataFactoryGenerator.TypeSelectionWrapper;
using TextCopy;

namespace PadyCli.ConsoleApp.Features.TestDataFactoryGeneration;

internal class TestDataFactoryGenerator
{
    private readonly IExternalAssemblyTestDataFactoryGenerator _externalAssemblyTestDataFactoryGenerator;
    private readonly PowershellCliRunner _cliRunner;

    public TestDataFactoryGenerator(
        IExternalAssemblyTestDataFactoryGenerator externalAssemblyTestDataFactoryGenerator,
        PowershellCliRunner cliRunner)
    {
        _externalAssemblyTestDataFactoryGenerator = externalAssemblyTestDataFactoryGenerator;
        _cliRunner = cliRunner;
    }

    public async Task<int> RunAsync(
        TestDataFactoryGenerationOptions options,
        Func<LogEventLevel> switchToVerboseAction,
        CancellationToken cancellationToken)
    {
        var generationParameters = new GenerationParameters(
            TestDataFactoryName: options.TestDataFactoryClassName,
            NameSpace: options.TestDataFactoryNamespace,
            TypeNames: options.CommaSepparatedClassNames.Split(',').ToImmutableHashSet(),
            OutputToConsole: true,
            WorkingDirectory: null,
            IncludeOptionalsCode: options.IncludeOptionals);

        if (options.Verbose)
        {
            switchToVerboseAction();
        }

        var lines = await _externalAssemblyTestDataFactoryGenerator
            .GenerateTestDataFactoryAsync(
                generationParameters: generationParameters,
                cliRunner: _cliRunner.Run,
                cancellationToken: cancellationToken);

        if (lines.Any())
        {
            await ClipboardService.SetTextAsync(string.Join("\n", lines), cancellationToken);
        }

        return 1;
    }
}