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
    
    public OperationResult ChangeLogLevel(LoggingLevelSwitch loggingLevelSwitch)
    {
        var logLevelChoice = AnsiConsole
            .Prompt(new SelectionPrompt<string>()
                .Title("Choose logLevel")
                .AddChoices(LogLevelVerbose, LogLevelDebug, LogLevelInfo, LogLevelWarning, LogLevelError, LogLevelFatal));

        LogEventLevel logLevel = logLevelChoice switch
        {
            LogLevelVerbose => LogEventLevel.Verbose,
            LogLevelDebug => LogEventLevel.Debug,
            LogLevelInfo => LogEventLevel.Information,
            LogLevelWarning => LogEventLevel.Warning,
            LogLevelError => LogEventLevel.Error,
            LogLevelFatal => LogEventLevel.Fatal,
            _ => throw new ArgumentException($"{logLevelChoice} not supported"),
        };
        
        loggingLevelSwitch.MinimumLevel = logLevel;
        
        return OperationResult.Continue;
    }
}