using System.Collections.Immutable;

namespace TestDataForTestDataFactoryGenerator.Served;

public record Delivery(IImmutableList<ItemPosition> Items, DateTimeOffset DeliveryDate);