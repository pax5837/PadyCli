using Microsoft.Extensions.DependencyInjection;
using PadyCliInteractiveConsoleApp;
using PadyCliInteractiveConsoleApp.Features;
using PadyCliInteractiveConsoleApp.Features.Docker;
using PadyCliInteractiveConsoleApp.Features.GuidGeneration;
using PadyCliInteractiveConsoleApp.Features.JsonSanitizer;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console;

const string GenerateOneGuid = "Generate a Guid";
const string SanitizeJsonText = "Sanitize JSON text in clipboard";
const string RunDockerContainer = "Run docker container";
const string Exit = "Exit";

var logLevelSwitch = new LoggingLevelSwitch();
logLevelSwitch.MinimumLevel = LogEventLevel.Information;
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(logLevelSwitch)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddSerilog())
    .AddFeatures()
    .BuildServiceProvider();

AnsiConsole.MarkupLine($"Welcome to pady's helper for sw development!");

while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("What do you want to do?")
            .AddChoices(
                GenerateOneGuid,
                SanitizeJsonText,
                RunDockerContainer,
                Exit));

    var operationResult = choice switch
    {
        Exit => OperationResult.ExitApplication,
        GenerateOneGuid => serviceProvider.GetService<GuidGenerator>()!.GenerateOneGuid(),
        SanitizeJsonText => serviceProvider.GetService<JsonSanitizerService>()!.Run(), 
        RunDockerContainer => serviceProvider.GetService<DockerService>()!.Run(),
        _ => ShowErrorAndContinue(choice),
    };

    if (operationResult == OperationResult.ExitApplication)
    {
        break;
    }
}

AnsiConsole.MarkupLine("[green]Goodbye![/]");

static OperationResult ShowErrorAndContinue(string choice)
{
    AnsiConsole.MarkupLine($"[red]Your selection '{choice}' is not supported, please try again[/]!");
    return OperationResult.Continue;
}