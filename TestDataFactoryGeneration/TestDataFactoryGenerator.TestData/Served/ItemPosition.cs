namespace TestDataFactoryGenerator.TestData.Served;

public record ItemPosition(Guid ItemPositionId, Guid ProductId, string ProductShortName, int ItemCount, decimal PositionPrice);