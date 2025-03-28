namespace TestDataForTestDataFactoryGenerator.Served;

public record ItemPosition(Guid ItemPositionId, Guid ProductId, string ProductShortName, int ItemCount, decimal PositionPrice);