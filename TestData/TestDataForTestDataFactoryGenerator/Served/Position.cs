namespace TestDataForTestDataFactoryGenerator.Served;

internal record Position(Guid PositionId, Guid ProductId, int Count, PositionNote? PositionNote, int? BatchSize);