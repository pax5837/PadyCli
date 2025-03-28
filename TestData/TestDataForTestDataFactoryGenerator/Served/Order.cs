using System.Collections.Immutable;

namespace TestDataForTestDataFactoryGenerator.Served;

internal record Order(Guid OrderId, DateTimeOffset OrderDate, IImmutableSet<Position> Position, ContactInfo ContactInfo, Delivery Delivery);