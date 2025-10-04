using System.Collections.Immutable;

namespace TestDataForTestDataFactoryGenerator.BusinessLogic;

public record Delivery(IImmutableList<ItemPosition> Items, DateTimeOffset DeliveryDate);