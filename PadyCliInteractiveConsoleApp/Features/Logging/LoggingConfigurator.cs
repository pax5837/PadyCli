using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace PadyCliInteractiveConsoleApp.Features.Logging;

internal static class LoggingConfigurator
{
    public static LoggingLevelSwitch Configure()
    {
        var logLevelSwitch = new LoggingLevelSwitch();
        logLevelSwitch.MinimumLevel = LogEventLevel.Information;
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(logLevelSwitch)
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
        
        return logLevelSwitch;
    }
}