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

            ("ImmutableArray", "ToImmutableArray()"),
        }.ToImmutableDictionary(x => x.CollectionTypeName, x => x.ToMethod);

    public bool IsACollection(Type type)
    {
        return CollectionMap.Keys.Any(type.Name.StartsWith);
    }

    public string GenerateInstantiationCode(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator)
    {
        var genericType = type.GenericTypeArguments[0];

        var typeName = CollectionMap.Keys.First(type.Name.StartsWith);
        dependencies.Add("System.Linq");

        return
            $"Enumerable.Range(1, _random.Next(0, 4)).Select(_ => {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(genericType, dependencies)}).{CollectionMap[typeName]}";
    }
}