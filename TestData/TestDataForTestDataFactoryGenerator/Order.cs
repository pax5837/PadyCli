using System.Collections.Immutable;

namespace TestDataForTestDataFactoryGenerator;

public record Order(Guid OrderId, IImmutableList<Item> Items);

public record Item(Guid ProductId, decimal UnitPrice);

public record Delivery(Item Item, DateTimeOffset DeliveryDate);