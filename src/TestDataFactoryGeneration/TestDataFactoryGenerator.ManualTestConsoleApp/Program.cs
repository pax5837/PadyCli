using TestDataFactoryGenerator;
using TestDataForTestDataFactoryGenerator;

var gen = Generator.GetNewGenerator();

var lines = gen.GenerateTestDataFactory("myTdf", "spacey", false, [typeof(Order)]);

foreach (var line in lines)
{
    Console.WriteLine(line);
}

Task.Delay(500).Wait();