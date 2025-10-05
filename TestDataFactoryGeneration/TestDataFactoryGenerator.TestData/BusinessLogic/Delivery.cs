using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.BusinessLogic;

public record Delivery(IImmutableList<ItemPosition> Items, DateTimeOffset DeliveryDate);