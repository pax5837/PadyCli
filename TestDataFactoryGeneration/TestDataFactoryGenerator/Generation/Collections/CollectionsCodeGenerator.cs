using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Collections;

internal class CollectionsCodeGenerator : ICollectionsCodeGenerator
{
    private readonly GenerationInformation _generationInformation;

    private static readonly IImmutableDictionary<string, string> CollectionMap =
        new (string CollectionTypeName, string ToMethod)[]
        {
            ("IImmutableList", string.Empty),
            ("ImmutableList", string.Empty),
            ("IReadOnlyList", string.Empty),
            ("List", string.Empty),

            ("IImmutableSet", string.Empty),
            ("ImmutableHashSet", string.Empty),
            ("ImmutableSortedSet", string.Empty),
            ("IReadOnlySet", "ToHashSet()"),
            ("FrozenSet", "ToFrozenSet()"),


            ("ImmutableArray", string.Empty),
            ("Array", string.Empty),
        }.ToImmutableDictionary(x => x.CollectionTypeName, x => x.ToMethod);

    private readonly string _leadingUnderscore;

    public CollectionsCodeGenerator(
        TdfGeneratorConfiguration config,
        GenerationInformation generationInformation)
    {
        _generationInformation = generationInformation;
        _leadingUnderscore = config.LeadingUnderscore();
    }

    public bool IsACollection(Type type)
    {
        return CollectionMap.Keys.Any(type.Name.StartsWith) || type.Name.EndsWith("[]");
    }

    public string GenerateInstantiationCode(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator)
    {
        var isArray = type.Name.EndsWith("[]");

        var genericType = isArray
            ? type.GetElementType()
            : type.GenericTypeArguments[0];

        dependencies.Add("System.Linq");

        var toMethod = CollectionMap[isArray ? "Array" :CollectionMap.Keys.First(type.Name.StartsWith)];

        _generationInformation.CollectionsGenerated = true;

        return
            string.IsNullOrEmpty(toMethod)
                ? $"[..GetSome(() => {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(genericType, dependencies)})]"
                : $"GetSome(() => {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(genericType, dependencies)}).{toMethod}";
    }
}