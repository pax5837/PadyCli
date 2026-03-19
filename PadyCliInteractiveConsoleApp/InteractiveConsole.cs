using Microsoft.Extensions.Logging;
using PadyCliInteractiveConsoleApp.Features.Docker;
using PadyCliInteractiveConsoleApp.Features.Greetings;
using PadyCliInteractiveConsoleApp.Features.GuidGeneration;
using PadyCliInteractiveConsoleApp.Features.JsonSanitizer;
using PadyCliInteractiveConsoleApp.Features.LineFiltering;
using PadyCliInteractiveConsoleApp.Features.Logging;
using Serilog.Core;
using Spectre.Console;
using PROTOUMLCONVERTER = PadyCliInteractiveConsoleApp.Features.ProtoToUmlConverter;
using TDFGENERATION = PadyCliInteractiveConsoleApp.Features.TestDataFactoryGeneration;

namespace PadyCliInteractiveConsoleApp;

internal class InteractiveConsole
{
    const string GenerateOneGuid = "Generate a Guid";
    const string SanitizeJsonText = "Sanitize JSON text in clipboard";
    const string FilterLines = "Filter lines from the text in the clipboard";
    const string RunDockerContainer = "Run docker container";
    const string GenerateUmlDiagramForProto = "Generate UML diagram for proto files";
    const string GenerateTdf = "Generate Test Data Factory for given types";
    const string ChangeLogLevel = "Change logging level of this application";
    const string Exit = "Exit";
    
    private readonly GreetingsService _greeter;
    private readonly GuidGenerator _guidGenerator;
    private readonly JsonSanitizerService _jsonSanitizer;
    private readonly LineFilteringService _lineFilteringService;
    private readonly DockerService _dockerService;
    private readonly PROTOUMLCONVERTER.ProtoToUmlConverter _protoToUmlConverter;
    private readonly TDFGENERATION.TestDataFactoryGenerator _testDataFactoryGenerator;
    private readonly LogLevelChanger _logLevelChanger;
    private readonly LoggingLevelSwitch _loggingLevelSwitch;
    private readonly ILogger<InteractiveConsole> _logger;

    public InteractiveConsole(
        GreetingsService greeter,
        GuidGenerator guidGenerator,
        JsonSanitizerService jsonSanitizer,
        LineFilteringService lineFilteringService,
        DockerService dockerService,
        PROTOUMLCONVERTER.ProtoToUmlConverter protoToUmlConverter,
        TDFGENERATION.TestDataFactoryGenerator testDataFactoryGenerator,
        LogLevelChanger logLevelChanger,
        LoggingLevelSwitch loggingLevelSwitch,
        ILogger<InteractiveConsole> logger)
    {
        _greeter = greeter;
        _guidGenerator = guidGenerator;
        _jsonSanitizer = jsonSanitizer;
        _lineFilteringService = lineFilteringService;
        _dockerService = dockerService;
        _protoToUmlConverter = protoToUmlConverter;
        _testDataFactoryGenerator = testDataFactoryGenerator;
        _logLevelChanger = logLevelChanger;
        _loggingLevelSwitch = loggingLevelSwitch;
        _logger = logger;
    }
    
    public void Run()
    {
        AnsiConsole.MarkupLine(_greeter.GetHelloMessage());

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What do you want to do?")
                    .AddChoices(
                        GenerateOneGuid,
                        SanitizeJsonText,
                        FilterLines,
                        RunDockerContainer,
                        GenerateUmlDiagramForProto,
                        GenerateTdf,
                        ChangeLogLevel,
                        Exit));
            _logger.LogTrace("Choice: {Choice}", choice);

            var operationResult = choice switch
            {
                Exit => OperationResult.ExitApplication,
                GenerateOneGuid => _guidGenerator.GenerateGuidInteractive(),
                SanitizeJsonText => _jsonSanitizer.Run(), 
                FilterLines => _lineFilteringService.Run(), 
                RunDockerContainer => _dockerService.Run(),
                GenerateUmlDiagramForProto => _protoToUmlConverter.Run(),
                GenerateTdf => _testDataFactoryGenerator.RunAsync(CancellationToken.None).GetAwaiter().GetResult(),
                ChangeLogLevel => _logLevelChanger.ChangeLogLevel(),
                _ => ShowErrorAndContinue(choice),
            };

            if (operationResult == OperationResult.ExitApplication)
            {
                break;
            }
        }

        AnsiConsole.MarkupLine($"[green]{_greeter.GetGoodbyeMessage()}[/]");

        static OperationResult ShowErrorAndContinue(string choice)
        {
            AnsiConsole.MarkupLine($"[red]Your selection '{choice}' is not supported, please try again[/]!");
            return OperationResult.Continue;
        }
    }
}