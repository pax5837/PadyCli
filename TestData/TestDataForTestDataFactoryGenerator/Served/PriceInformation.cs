using System.Collections.Immutable;

namespace TestDataForTestDataFactoryGenerator.Served;

public record PriceInformation(decimal BasePrice, IImmutableDictionary<int, decimal> DiscountPercentByCount);