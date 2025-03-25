using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace TestDataFactoryGenerator;

public record SimpleTypeConfiguration(
    string ParameterNamePlaceholder,
    ImmutableList<InstantiationConfigurationForType> InstantiationConfigurations)
{
    [JsonIgnore]
    public IImmutableDictionary<Type, InstantiationConfigurationForType> InstantiationConfigurationForTypeByType
        => InstantiationConfigurations.ToImmutableDictionary(x => x.Type, x => x);
}