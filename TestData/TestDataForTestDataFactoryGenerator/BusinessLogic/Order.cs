using System.Collections.Immutable;

namespace TestDataForTestDataFactoryGenerator.BusinessLogic;

public record Order(Guid OrderId, IImmutableList<ItemPosition> Items);