namespace TestDataForTestDataFactoryGenerator.BusinessLogic;

public record ItemPosition(Guid ItemPositionId, Guid ProductId, int Count, decimal UnitPrice);