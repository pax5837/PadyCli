using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.Served;

internal record Order(
    Guid OrderId,
    DateTimeOffset OrderDate,
    IImmutableSet<Position> Position,
    ContactInfo ContactInfo,
    Delivery Delivery,
    IImmutableDictionary<Guid, PriceInformation> PriceByProductId,
    AllCollections AllCollections);