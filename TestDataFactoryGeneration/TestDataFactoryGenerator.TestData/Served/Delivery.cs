using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.Served;

public record Delivery(IImmutableList<ItemPosition> Items, DateTimeOffset DeliveryDate)
{
    public Delivery(IImmutableList<ItemPosition> items) : this(items, DateTimeOffset.Now)
    {
    }
}