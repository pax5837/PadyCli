using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Dictionnaries;

internal class DictionnariesCodeGenerator : IDictionnariesCodeGenerator
{
    private readonly GenerationInformation _generationInformation;

    private static readonly ImmutableDictionary<string, string> DictionaryMap =
        new (string CollectionTypeName, string ToMethod)[]
        {
            ("IImmutableDictionary", "ToImmutableDictionary(x => x.Key, x => x.Value)"),
            ("ImmutableDictionary", "ToImmutableDictionary(x => x.Key, x => x.Value)"),
            ("ImmutableSortedDictionary", "ToImmutableSortedDictionary(x => x.Key, x => x.Value)"),
            ("FrozenDictionary", "ToFrozenDictionary(x => x.Key, x => x.Value)"),
            ("Dictionary", "ToDictionary(x => x.Key, x => x.Value)"),
        }.ToImmutableDictionary(x => x.CollectionTypeName, x => x.ToMethod);

    private readonly string _leadingUnderscore;

    public DictionnariesCodeGenerator(
        TdfGeneratorConfiguration config,
        GenerationInformation generationInformation)
    {
        _leadingUnderscore = config.LeadingUnderscore();
        _generationInformation = generationInformation;
    }

    public bool IsADictionary(Type type)
    {
        return DictionaryMap.Keys.Any(type.Name.StartsWith);
    }

    public string GenerateInstantiationCode(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator)
    {
        var typeName = DictionaryMap.Keys.First(type.Name.StartsWith);
        var keyType = type.GenericTypeArguments[0];
        var valueType = type.GenericTypeArguments[1];

        _generationInformation.CollectionsGenerated = true;

        return $"GetSome(() => (Key: {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(keyType, dependencies)}, Value: {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(valueType, dependencies)})).{DictionaryMap[typeName]}";
    }
}