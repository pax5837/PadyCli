﻿using CommandLine;
using CsProjMover;
using DotnetInfrastructure;
using Infrastructure.DebugServices;
using Microsoft.Extensions.DependencyInjection;
using PadyCli.ConsoleApp.Features;
using PadyCli.ConsoleApp.Features.About;
using PadyCli.ConsoleApp.Features.About.Implementations;
using PadyCli.ConsoleApp.Features.CsProjectMover;
using PadyCli.ConsoleApp.Features.Docker;
using PadyCli.ConsoleApp.Features.GuidGeneration;
using PadyCli.ConsoleApp.Features.ProtoToUmlConverter;
using PadyCli.ConsoleApp.Features.TestClassGeneration;
using PadyCli.ConsoleApp.Features.TestDataFactoryGeneration;
using ProtoToUmlConverter;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using TestDataFactoryGenerator;
using TestDataFactoryGenerator.TypeSelectionWrapper;
using TestingHelpers;

var logLevelSwitch = new LoggingLevelSwitch();
logLevelSwitch.MinimumLevel = LogEventLevel.Information;
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.ControlledBy(logLevelSwitch)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var switchToVerboseAction = () => logLevelSwitch.MinimumLevel = LogEventLevel.Verbose;

var wd = Environment.CurrentDirectory;

var serviceProvider = new ServiceCollection()
    .AddLogging(builder => builder.AddSerilog())
    .AddAbout()
    .AddFeatures()
    .AddDebugServices()
    .AddDotnetInfrastructure()
    .AddTestHelpers(new TestClassGeneratorConfig(Indent: "    "))
    .AddProtoToUmlServices()
    .AddProjectMoverServices()
    .AddExternalAssemblyTestDataFactoryGeneration(BuildTdfGeneratorConfig())
    .BuildServiceProvider();

var fileSystemOpsForbiddenEnvVariable = Environment.GetEnvironmentVariable("FILESYSTEMOPSFORBIDDEN");
serviceProvider.GetService<IDebugServiceInitializer>()!.FileSystemModificationsAllowed =
    fileSystemOpsForbiddenEnvVariable != "true";

var parserResult = Parser.Default
    .ParseArguments<
        TestClassGeneratorOptions,
        GuidGeneratorOptions,
        ProtoConverterOptions,
        CsProjectMoverOptions,
        TestDataFactoryGenerationOptions,
        DockerOptions,
        AboutOptions>(args);

await parserResult.WithParsedAsync(async (TestClassGeneratorOptions opts)
    => await serviceProvider.GetService<TestClassGeneratorAdapter>()!.RunAsync(
        opts,
        switchToVerboseAction,
        CancellationToken.None));

await parserResult.WithParsedAsync(async (TestDataFactoryGenerationOptions opts)
    => await serviceProvider.GetService<PadyCli.ConsoleApp.Features.TestDataFactoryGeneration.TestDataFactoryGenerator>()!.RunAsync(
        opts,
        switchToVerboseAction,
        CancellationToken.None));

parserResult.WithParsed((GuidGeneratorOptions opts)
    => serviceProvider.GetService<GuidGenerator>()!.Run(opts));

parserResult.WithParsed((ProtoConverterOptions opts)
    => serviceProvider.GetService<ProtToUmlConverter>()!.Run(opts));

parserResult.WithParsed((CsProjectMoverOptions opts)
    => serviceProvider.GetService<CsProjectMoverAdapter>()!.Run(opts));

parserResult.WithParsed((DockerOptions opts)
    => serviceProvider.GetService<DockerService>()!.Run(opts));

await parserResult.WithParsedAsync((AboutOptions opts)
    => serviceProvider.GetService<About>()!.RunAsync(opts));

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