using TestDataFactoryGenerator.TestData.SharedStuff;

namespace TestDataFactoryGenerator.TestData.Served;

internal record Person(Guid Id, string FirstName, string LastName, Address Address);