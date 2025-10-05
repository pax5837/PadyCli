using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.Served;

public record PriceInformation(decimal BasePrice, IImmutableDictionary<int, decimal> DiscountPercentByCount);