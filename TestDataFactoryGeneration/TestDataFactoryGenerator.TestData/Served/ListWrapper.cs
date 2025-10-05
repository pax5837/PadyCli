namespace TestDataFactoryGenerator.TestData.Served;

public record ListWrapper<T1, T2>(string Name, List<T1> Items1, List<T2> Items2);