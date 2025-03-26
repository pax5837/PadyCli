using DotnetInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using TestDataFactoryGenerator;
using TestDataFactoryGenerator.TypeSelectionWrapper;
using TestDataForTestDataFactoryGenerator.BusinessLogic;

// GenerateAndPrintTestDataTdfGeneratorVariant1();
await GenerateAndPrintTestDataTdfGeneratorVariant2Async(CancellationToken.None);

Task.Delay(500).Wait();

void GenerateAndPrintTestDataTdfGeneratorVariant1()
{
    var gen = TdfGeneratorFactory.GetNew();

    var lines = gen.GenerateTestDataFactory("myTdf", "spacey", false, [typeof(Order), typeof(Delivery)]);

    foreach (var line in lines)
    {
        Console.WriteLine(line);
    }
}

async Task GenerateAndPrintTestDataTdfGeneratorVariant2Async(CancellationToken cancellationToken)
{
    var tdfGeneratorConfiguration = BuildTdfGeneratorConfig();
    var tdfGeneratorConfigurationOrPathToJson = new TdfGeneratorConfigurationOrPathToJson(tdfGeneratorConfiguration);
    
    var services = new ServiceCollection()
        .AddLogging()
        .AddDotnetInfrastructure()
        .AddExternalAssemblyTestDataFactoryGeneration(tdfGeneratorConfigurationOrPathToJson)
        .BuildServiceProvider();

    var generator = services.GetRequiredService<IExternalAssemblyTestDataFactoryGenerator>();

    var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
    var executionDirectory = Path.Combine(
        currentDirectory.Parent.Parent.Parent.Parent.Parent.FullName,
        "TestData\\TestDataForTestDataFactoryGenerator");

    var generationParameters = new GenerationParameters(
        TestDataFactoryName: "MyTdf",
        NameSpace: "sapcey",
        TypeNames: ["Order", "Delivery"],
        OutputToConsole: false,
        WorkingDirectory: executionDirectory);
    var lines = await generator.GenerateTestDataFactoryAsync(generationParameters, cancellationToken);

    foreach (var line in lines)
    {
        Console.WriteLine(line);
    }
}

TdfGeneratorConfiguration BuildTdfGeneratorConfig()
{
    var simpleTypeConfiguration = new SimpleTypeConfiguration(
        "#########",
        [
            new(typeof(string), "GenerateRandomString(\"#########\")", "System.String", ["private string GenerateRandomString(string? parameterName) => $\"{parameterName ?? \"SomeString\"}_{_random.Next(1, int.MaxValue)}\";"]),
            new(typeof(int), "GenerateRandomInt()", null, ["private int GenerateRandomInt() => _random.Next();"]),
            new(typeof(Guid), "GenerateRandomGuid()", null, ["private Guid GenerateRandomGuid() => Guid.NewGuid();"]),
            new(typeof(DateTimeOffset), "GenerateRandomDateTimeOffset()", null, ["private DateTimeOffset GenerateRandomDateTimeOffset() => new DateTimeOffset(_random.NextInt64(), TimeSpan.FromHours(_random.Next(-23, 23)));"]),
            new(typeof(TimeSpan), "GenerateRandomTimeSpan()", null, ["private TimeSpan GenerateRandomTimeSpan() => new TimeSpan(_random.NextInt64());"]),
            new(typeof(bool), "GenerateRandomBool()", null, ["private bool GenerateRandomBool() => _random.Next() % 2 == 0;"]),
            new(typeof(long), "GenerateRandomLong()", null, ["private long GenerateRandomLong() => _random.NextInt64();"]),
            new(typeof(decimal), "GenerateRandomDecimal()", null, ["private decimal GenerateRandomDecimal() => (decimal)_random.NextDouble();"]),
        ]);

    var tdfGeneratorConfiguration1 = new TdfGeneratorConfiguration(
        NamespacesToAdd: [],
        Indent: "    ",
        EitherNamespace: null,
        CustomInstantiationForWellKnownProtobufTypes: [],
        SimpleTypeConfiguration: simpleTypeConfiguration);
    return tdfGeneratorConfiguration1;
}