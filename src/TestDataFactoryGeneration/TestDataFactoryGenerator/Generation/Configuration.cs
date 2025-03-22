using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace TestDataFactoryGenerator.Generation;

public class Configuration : IConfiguration
{
    [JsonInclude]
    public IImmutableSet<string> NamespacesToAdd { get; }

    [JsonInclude]
    public string? EitherNamespace { get; }

    [JsonInclude]
    public IImmutableList<CustomInstantiationInfo> CustomInstantiationInfo { get; }

    [JsonIgnore]
    public IImmutableDictionary<string, CustomInstantiationInfo> CustomInstantiationInfoByFullName { get; }

    [JsonConstructor]
    public Configuration(
        IImmutableSet<string> namespacesToAdd,
        string eitherNameSpace,
        IImmutableList<CustomInstantiationInfo> customInstantiationInfo)
    {
        NamespacesToAdd = namespacesToAdd;
        EitherNamespace = eitherNameSpace;
        CustomInstantiationInfo = customInstantiationInfo;
        CustomInstantiationInfoByFullName = customInstantiationInfo.ToImmutableDictionary(x => x.TypeFullName, x => x);
    }

}