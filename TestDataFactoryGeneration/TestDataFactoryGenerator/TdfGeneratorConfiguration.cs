using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace TestDataFactoryGenerator;

public record TdfGeneratorConfiguration(
    IImmutableSet<string> NamespacesToAdd,
    string Indent,
    string? EitherNamespace,
    IImmutableList<InstantiationConfigurationForName> CustomInstantiationForWellKnownProtobufTypes,
    SimpleTypeConfiguration SimpleTypeConfiguration,
    bool UseLeadingUnderscoreForPrivateFields)
{
    [JsonIgnore]
    public IImmutableDictionary<string, InstantiationConfigurationForName>
        CustomInstantiationForWellKnownProtobufTypesByFullName
        => CustomInstantiationForWellKnownProtobufTypes.ToImmutableDictionary(x => x.TypeFullName, x => x);
}