using DotnetInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using TestDataFactoryGenerator;
using TestDataFactoryGenerator.Generation;
using TestDataFactoryGenerator.TypeSelectionWrapper;
using TestDataForTestDataFactoryGenerator.BusinessLogic;

// GenerateAndPrintTestDataTdfGeneratorVariant1();
await GenerateAndPrintTestDataTdfGeneratorVariant2Async(CancellationToken.None);

Task.Delay(500).Wait();

void GenerateAndPrintTestDataTdfGeneratorVariant1()
{
    var gen = Generator.GetNewGenerator();

    var lines = gen.GenerateTestDataFactory("myTdf", "spacey", false, [typeof(Order), typeof(Delivery)]);

    foreach (var line in lines)
    {
        Console.WriteLine(line);
    }
}

async Task GenerateAndPrintTestDataTdfGeneratorVariant2Async(CancellationToken cancellationToken)
{
    var services = new ServiceCollection()
        .AddLogging()
        .AddDotnetInfrastructure()
        .AddExternalAssemblyTestDataFactoryGeneration(new Configuration([],null, []))
        .BuildServiceProvider();

    var generator = services.GetRequiredService<IExternalAssemblyTestDataFactoryGenerator>();

    var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
    var executionDirectory = Path.Combine(
        currentDirectory.Parent.Parent.Parent.Parent.Parent.Parent.FullName,
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