namespace TestDataForTestDataFactoryGenerator.BusinessLogic;

public record ItemPosition(Guid ItemPositionId, Guid ProductId, string ProductShortName, int Count, decimal UnitPrice);