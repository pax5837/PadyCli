using DotnetInfrastructure;
using Infrastructure.DebugServices;
using Microsoft.Extensions.DependencyInjection;
using PadyCliInteractiveConsoleApp;
using PadyCliInteractiveConsoleApp.Features;
using PadyCliInteractiveConsoleApp.Features.Logging;
using PadyCliInteractiveConsoleApp.Features.TestDataFactoryGeneration;
using ProtoToUmlConverter;
using Serilog;
using TestDataFactoryGenerator.TypeSelectionWrapper;


var logLevelSwitch = LoggingConfigurator.Configure();

var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddSerilog())
    .AddFeatures()
    .AddProtoToUmlServices()
    .AddDebugServices()
    .AddDotnetInfrastructure()
    .AddExternalAssemblyTestDataFactoryGeneration(TestDataFactoryConfiguration.GetTdfConfig())
    .AddSingleton<InteractiveConsole>()
    .AddSingleton<ArgBasedRunner>()
    .AddSingleton(logLevelSwitch)
    .BuildServiceProvider();

if (args.Length == 0)
{
    serviceProvider.GetRequiredService<InteractiveConsole>().Run();
}
else
{
    serviceProvider.GetRequiredService<ArgBasedRunner>().Run(args);
}




