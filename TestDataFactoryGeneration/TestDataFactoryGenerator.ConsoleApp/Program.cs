using Microsoft.Extensions.DependencyInjection;
using TestDataFactoryGenerator;

Action<IServiceCollection> loggingRegistration = (IServiceCollection services) => services.AddLogging();

var testDataFactoryGenerator = TdfGeneratorFactory.GetNew(loggingRegistration: loggingRegistration);

var lines = testDataFactoryGenerator.GenerateTestDataFactory(
    "TdfName",
    "TdfDescription",
    true,
    [
        typeof(TestDataFactoryGenerator.TestData.Served.AllCollections),
        typeof(TestDataFactoryGenerator.TestData.Served.Order),
        typeof(TestDataFactoryGenerator.TestData.BusinessLogic.Order),
    ],
    true);