namespace TestDataFactoryGenerator.TestData.BusinessLogic;

public record ItemPosition(Guid ItemPositionId, Guid ProductId, string ProductShortName, int Count, decimal UnitPrice);