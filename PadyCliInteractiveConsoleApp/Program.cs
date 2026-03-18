using DotnetInfrastructure;
using Infrastructure.DebugServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PadyCliInteractiveConsoleApp;
using PadyCliInteractiveConsoleApp.Features;
using PadyCliInteractiveConsoleApp.Features.Docker;
using PadyCliInteractiveConsoleApp.Features.Greetings;
using PadyCliInteractiveConsoleApp.Features.GuidGeneration;
using PadyCliInteractiveConsoleApp.Features.JsonSanitizer;
using PadyCliInteractiveConsoleApp.Features.LineFiltering;
using PadyCliInteractiveConsoleApp.Features.Logging;
using ProtoToUmlConverter;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console;
using TestDataFactoryGenerator;
using TestDataFactoryGenerator.TypeSelectionWrapper;

const string GenerateOneGuid = "Generate a Guid";
const string SanitizeJsonText = "Sanitize JSON text in clipboard";
const string FilterLines = "Filter lines from the text in the clipboard";
const string RunDockerContainer = "Run docker container";
const string GenerateUmlDiagramForProto = "Generate UML diagram for proto files";
const string GenerateTdf = "Generate Test Data Factory for given types";
const string ChangeLogLevel = "Change logging level of this application";
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
    .AddProtoToUmlServices()
    .AddDebugServices()
    .AddDotnetInfrastructure()
    .AddExternalAssemblyTestDataFactoryGeneration(BuildTdfGeneratorConfig())
    .BuildServiceProvider();

var greeter = serviceProvider.GetService<GreetingsService>()!;
var logger = serviceProvider.GetService<ILogger<Program>>()!;

AnsiConsole.MarkupLine(greeter.GetHelloMessage());

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
    logger.LogTrace("Choice: {Choice}", choice);

    var operationResult = choice switch
    {
        Exit => OperationResult.ExitApplication,
        GenerateOneGuid => serviceProvider.GetService<GuidGenerator>()!.GenerateOneGuid(),
        SanitizeJsonText => serviceProvider.GetService<JsonSanitizerService>()!.Run(), 
        FilterLines => serviceProvider.GetService<LineFilteringService>()!.Run(), 
        RunDockerContainer => serviceProvider.GetService<DockerService>()!.Run(),
        GenerateUmlDiagramForProto => serviceProvider.GetService<PadyCliInteractiveConsoleApp.Features.ProtoToUmlConverter.ProtoToUmlConverter>()!.Run(),
        GenerateTdf => serviceProvider.GetService<PadyCliInteractiveConsoleApp.Features.TestDataFactoryGeneration.TestDataFactoryGenerator>()!.RunAsync(CancellationToken.None).GetAwaiter().GetResult(),
        ChangeLogLevel => serviceProvider.GetService<LogLevelChanger>()!.ChangeLogLevel(logLevelSwitch),
        _ => ShowErrorAndContinue(choice),
    };

    if (operationResult == OperationResult.ExitApplication)
    {
        break;
    }
}

AnsiConsole.MarkupLine($"[green]{greeter.GetGoodbyeMessage()}[/]");

static OperationResult ShowErrorAndContinue(string choice)
{
    AnsiConsole.MarkupLine($"[red]Your selection '{choice}' is not supported, please try again[/]!");
    return OperationResult.Continue;
}

TdfConfigDefinition BuildTdfGeneratorConfig()
{
    var useLeadingUnderscoreForPrivateFields = true;

    var randomField = useLeadingUnderscoreForPrivateFields ? "_random" : "random";


    var simpleTypeConfiguration = new SimpleTypeConfiguration(
        ParameterNamePlaceholder: "#########",
        InstantiationConfigurations:
        [
            new(typeof(string), $"{randomField}.NextString()", "System.String", []),
            new(typeof(int), $"{randomField}.Next()", null, []),
            new(typeof(Guid), $"{randomField}.NextGuid()", null, []),
            new(typeof(DateTimeOffset), $"{randomField}.NextDateTimeOffset()", null, []),
            new(typeof(DateTime), $"{randomField}.NextDateTime()", null, []),
            new(typeof(TimeSpan), $"{randomField}.NextTimeSpan()", null, []),
            new(typeof(bool), $"{randomField}.NextBool()", null, []),
            new(typeof(long), $"{randomField}.NextInt64()", null, []),
            new(typeof(decimal), $"{randomField}.NextDecimal()", null, []),
        ]);

    var tdfGeneratorConfiguration = new TdfGeneratorConfiguration(
        NamespacesToAdd: [],
        Indent: "\t",
        EitherNamespace: null,
        CustomInstantiationForWellKnownProtobufTypes: [],
        SimpleTypeConfiguration: simpleTypeConfiguration,
        UseLeadingUnderscoreForPrivateFields: useLeadingUnderscoreForPrivateFields);
    return TdfConfigDefinition.FromConfig(tdfGeneratorConfiguration);
}