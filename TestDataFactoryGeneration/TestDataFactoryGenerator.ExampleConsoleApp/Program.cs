using System.Collections.Immutable;
using DotnetInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using TestDataFactoryGenerator;
using TestDataFactoryGenerator.TypeSelectionWrapper;
using TextCopy;

Console.WriteLine("Use custom config? [y/n]");
var useCustomConfig = Console.ReadLine()?.ToLowerInvariant() == "y";
var config = useCustomConfig ? BuildTdfGeneratorConfig() : null;
await GenerateAndPrintTestDataTdfGeneratorVariant2Async(config, CancellationToken.None);

Task.Delay(500).Wait();


async Task GenerateAndPrintTestDataTdfGeneratorVariant2Async(TdfConfigDefinition? config, CancellationToken cancellationToken)
{
    var services = new ServiceCollection()
        .AddLogging()
        .AddDotnetInfrastructure()
        .AddExternalAssemblyTestDataFactoryGeneration(config)
        .BuildServiceProvider();

    var generator = services.GetRequiredService<IExternalAssemblyTestDataFactoryGenerator>();

    var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
    var executionDirectory = Path.Combine(
        currentDirectory.Parent.Parent.Parent.Parent.Parent.FullName,
        "TestDataFactoryGeneration\\TestDataFactoryGenerator.TestData");

    var generationParameters = new GenerationParameters(
        TestDataFactoryName: "Tdf",
        NameSpace: "TestDataFactoryGenerator.TestData",
        TypeNames: ["Order", "Delivery"],
        OutputToConsole: false,
        WorkingDirectory: executionDirectory,
        IncludeOptionalsCode: true);
    var lines = await generator.GenerateTestDataFactory(generationParameters, cancellationToken);

    foreach (var line in lines)
    {
        Console.WriteLine(line);
    }

    ClipboardService.SetText(string.Join(Environment.NewLine, lines));
}

TdfConfigDefinition BuildTdfGeneratorConfig()
{
    var useLeadingUnderscoreForPrivateFields = false;

    var randomField = useLeadingUnderscoreForPrivateFields ? "_random" : "random";

    ImmutableList<InstantiationConfigurationForType> instantiationConfigurationForTypes =
    [
        new(typeof(string), $"{randomField}.NextString(\"#########\")", "System.String", []),
        new(typeof(int), $"{randomField}.Next()", null, []),
        new(typeof(Guid), $"{randomField}.NextGuid()", null, []),
        new(typeof(DateTimeOffset), $"{randomField}.NextDateTimeOffset()", null, []),
        new(typeof(DateTime), $"{randomField}.NextDateTime()", null, []),
        new(typeof(TimeSpan), $"{randomField}.NextTimeSpan()", null, []),
        new(typeof(bool), $"{randomField}.NextBool()", null, []),
        new(typeof(long), $"{randomField}.NextLong()", null, []),
        new(typeof(decimal), $"{randomField}.NextDecimal()", null, []),
    ];

    var simpleTypeConfiguration = new SimpleTypeConfiguration(
        "#########",
        instantiationConfigurationForTypes
            .Select(selector:
                x => x with
                {
                    MethodCodeToAdd = x.MethodCodeToAdd
                        .Select(selector: c => useLeadingUnderscoreForPrivateFields ? c : c.Replace(oldValue: "_random", newValue: "random"))
                        .ToImmutableList()
                })
            .ToImmutableList());

    return TdfConfigDefinition.FromConfig(
        new TdfGeneratorConfiguration(
            NamespacesToAdd: [],
            Indent: "    ",
            EitherNamespace: null,
            CustomInstantiationForWellKnownProtobufTypes: [],
            SimpleTypeConfiguration: simpleTypeConfiguration,
            UseLeadingUnderscoreForPrivateFields: useLeadingUnderscoreForPrivateFields));
}