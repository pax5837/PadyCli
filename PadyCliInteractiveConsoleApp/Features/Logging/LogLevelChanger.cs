using System.Collections.Immutable;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console;

namespace PadyCliInteractiveConsoleApp.Features.Logging;

internal class LogLevelChanger
{
    private const string LogLevelVerbose = "Verbose";
    private const string LogLevelDebug = "Debug";
    private const string LogLevelInfo = "Info";
    private const string LogLevelWarning = "Warning";
    private const string LogLevelError = "Error";
    private const string LogLevelFatal = "Fatal";

    private static readonly IImmutableSet<(string LogLevelString, LogEventLevel LogLevelEnum)> LogLevels =
    [
        (LogLevelVerbose, LogEventLevel.Verbose),
        (LogLevelDebug, LogEventLevel.Debug),
        (LogLevelInfo, LogEventLevel.Information),
        (LogLevelWarning, LogEventLevel.Warning),
        (LogLevelError, LogEventLevel.Error),
        (LogLevelFatal, LogEventLevel.Fatal)
    ];

    private static IImmutableDictionary<string, LogEventLevel> LogLevelEnumByString
        = LogLevels.ToImmutableDictionary(x => x.LogLevelString, x => x.LogLevelEnum);

    private static IImmutableDictionary<LogEventLevel, string> LogLevelStringByEnum
        = LogLevels.ToImmutableDictionary(x => x.LogLevelEnum, x => x.LogLevelString);

    public OperationResult ChangeLogLevel(LoggingLevelSwitch loggingLevelSwitch)
    {
        var logLevelChoice = AnsiConsole
            .Prompt(new SelectionPrompt<string>()
                .Title($"Choose logLevel (current: {LogLevelStringByEnum[loggingLevelSwitch.MinimumLevel]})")
                .AddChoices(LogLevelVerbose, LogLevelDebug, LogLevelInfo, LogLevelWarning, LogLevelError, LogLevelFatal));

        loggingLevelSwitch.MinimumLevel = LogLevelEnumByString[logLevelChoice];
        
        return OperationResult.Continue;
    }
}