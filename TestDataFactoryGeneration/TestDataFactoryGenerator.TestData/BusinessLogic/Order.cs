using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.BusinessLogic;

public record Order(Guid OrderId, IImmutableList<ItemPosition> Items);