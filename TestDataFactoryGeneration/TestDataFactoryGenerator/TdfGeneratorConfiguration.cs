using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace TestDataFactoryGenerator;

public record TdfGeneratorConfiguration(
    IImmutableSet<string>  NamespacesToAdd,
    string? EitherNamespace,
    IImmutableList<InstantiationConfiguration> CustomInstantiationForWellKnownProtobufTypes,
    SimpleTypeConfiguration SimpleTypeConfiguration)
{
    [JsonIgnore]
    public IImmutableDictionary<string, InstantiationConfiguration> CustomInstantiationForWellKnownProtobufTypesByFullName => CustomInstantiationForWellKnownProtobufTypes.ToImmutableDictionary(x => x.TypeFullName, x => x);
}