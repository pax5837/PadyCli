using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.Served;

public record Delivery(IImmutableList<ItemPosition> Items, DateTimeOffset DeliveryDate);