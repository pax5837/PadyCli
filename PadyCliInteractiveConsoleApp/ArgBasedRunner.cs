using CommandLine;
using PadyCliInteractiveConsoleApp.Features.GuidGeneration;
using PadyCliInteractiveConsoleApp.Features.JsonSanitizer;

namespace PadyCliInteractiveConsoleApp;

internal class ArgBasedRunner
{
    private readonly GuidGenerator _guidGenerator;
    private readonly JsonSanitizerService _jsonSanitizerService;
    private readonly InteractiveConsole _interactiveConsole;

    public ArgBasedRunner(GuidGenerator guidGenerator, JsonSanitizerService jsonSanitizerService, InteractiveConsole interactiveConsole)
    {
        _guidGenerator = guidGenerator;
        _jsonSanitizerService = jsonSanitizerService;
        _interactiveConsole = interactiveConsole;
    }
    
    public void Run(string[] args)
    {
        var parserResult = Parser.Default
            .ParseArguments<
                GuidGeneratorOptions,
                JsonSanitizerOptions,
                InteractiveConsoleOptions>(args);
    
        parserResult.WithParsed((GuidGeneratorOptions opts) => _guidGenerator.Run(opts));
        parserResult.WithParsed((JsonSanitizerOptions opts) => _jsonSanitizerService.Run(opts));   
        parserResult.WithParsed((InteractiveConsoleOptions opts) => _interactiveConsole.Run());   
    }
}

[Verb("interactive", aliases: ["i"] , HelpText = "Runs in interactive mode with more advanced functions. Is also the default when no verb is specified.")]
public class InteractiveConsoleOptions
{ }