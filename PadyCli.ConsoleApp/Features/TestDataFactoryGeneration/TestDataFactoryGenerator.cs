using System.Collections.Immutable;
using Serilog.Events;
using TestDataFactoryGenerator.TypeSelectionWrapper;
using TextCopy;

namespace PadyCli.ConsoleApp.Features.TestDataFactoryGeneration;

internal class TestDataFactoryGenerator
{
    private readonly IExternalAssemblyTestDataFactoryGenerator _externalAssemblyTestDataFactoryGenerator;

    public TestDataFactoryGenerator(IExternalAssemblyTestDataFactoryGenerator externalAssemblyTestDataFactoryGenerator)
    {
        _externalAssemblyTestDataFactoryGenerator = externalAssemblyTestDataFactoryGenerator;
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
            .GenerateTestDataFactoryAsync(generationParameters, cancellationToken);

        ClipboardService.SetText(string.Join("\n", lines));

        return 1;
    }
}