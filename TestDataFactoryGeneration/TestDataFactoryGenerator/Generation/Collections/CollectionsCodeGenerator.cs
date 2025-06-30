using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Collections;

internal class CollectionsCodeGenerator : ICollectionsCodeGenerator
{
    private static readonly IImmutableDictionary<string, string> CollectionMap =
        new (string CollectionTypeName, string ToMethod)[]
        {
            ("IImmutableList", "ToImmutableList()"),
            ("ImmutableList", "ToImmutableList()"),
            ("IReadOnlyList", "ToList()"),
            ("List", "ToList()"),

            ("IImmutableSet", "ToImmutableHashSet()"),
            ("ImmutableHashSet", "ToImmutableHashSet()"),
            ("ImmutableSortedSet", "ToImmutableSortedSet()"),
            ("IReadOnlySet", "ToHashSet()"),
            ("FrozenSet", "ToFrozenSet()"),


            ("ImmutableArray", "ToImmutableArray()"),
            ("Array", "ToArray()"),
        }.ToImmutableDictionary(x => x.CollectionTypeName, x => x.ToMethod);

    private readonly string _leadingUnderscore;

    public CollectionsCodeGenerator(TdfGeneratorConfiguration config)
    {
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

        var toMethod = isArray
            ? "ToArray()"
            : CollectionMap[CollectionMap.Keys.First(type.Name.StartsWith)];

        return
            $"Enumerable.Range(1, {_leadingUnderscore}random.Next(0, GetZeroBiasedCount())).Select(_ => {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(genericType, dependencies)}).{toMethod}";
    }
}