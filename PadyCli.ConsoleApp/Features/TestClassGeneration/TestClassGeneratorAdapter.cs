using Serilog.Events;
using TestingHelpers;
using TextCopy;

namespace PadyCli.ConsoleApp.Features.TestClassGeneration;

internal class TestClassGeneratorAdapter
{
    private readonly ITestClassGenerator _testClassGenerator;

    public TestClassGeneratorAdapter(ITestClassGenerator testClassGenerator)
    {
        this._testClassGenerator = testClassGenerator;
    }

    public async Task<int> RunAsync(
        TestClassGeneratorOptions options,
        Func<LogEventLevel> switchToVerboseAction,
        CancellationToken cancellationToken)
    {
        if (options.Verbose)
        {
            switchToVerboseAction();
        }

        var lines = await _testClassGenerator.GenerateTestClassAsync(
            options.ClassName,
            cancellationToken);

        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }

        ClipboardService.SetText(string.Join("\n", lines));

        return 1;
    }
}