using System.Collections.Immutable;

namespace TestDataForTestDataFactoryGenerator.Boundaries.Served;

internal record Order(Guid OrderId, DateTimeOffset OrderDate, IImmutableSet<Position> Positions);