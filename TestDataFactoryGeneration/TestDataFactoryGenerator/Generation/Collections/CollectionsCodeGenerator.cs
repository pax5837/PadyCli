using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Collections;

internal class CollectionsCodeGenerator : ICollectionsCodeGenerator
{
    private readonly GenerationInformation _generationInformation;
    private readonly INamespaceAliasManager _namespaceAliasManager;

    private static readonly IImmutableDictionary<string, Instantiation?> CollectionMap =
        new (string CollectionTypeName, Instantiation? Instantiation)[]
        {
            ("IImmutableList", null),
            ("ImmutableList", null),
            ("IReadOnlyList", null),
            ("List", null),
            ("LinkedList", new Instantiation(InstantiationMethod.ConstructionMethod, "new LinkedList")),

            ("ISet", new Instantiation(InstantiationMethod.ConstructionMethod, "new HashSet")),
            ("IImmutableSet", null),
            ("ImmutableHashSet", null),
            ("ImmutableSortedSet", null),
            ("IReadOnlySet", new Instantiation(InstantiationMethod.ToMethod, "ToHashSet()")),
            ("FrozenSet", new Instantiation(InstantiationMethod.ToMethod, "ToFrozenSet()")),
            ("SortedSet", null),

            ("ImmutableArray", null),
            ("Array", null),

            ("ICollection", null),
            ("IReadOnlyCollection", null),

            ("IEnumerable", null),

            ("Queue", new Instantiation(InstantiationMethod.ConstructionMethod, "new Queue")),
            ("Stack", new Instantiation(InstantiationMethod.ConstructionMethod, "new Stack")),
        }.ToImmutableDictionary(x => x.CollectionTypeName, x => x.Instantiation);

    private readonly string _leadingUnderscore;

    public CollectionsCodeGenerator(
        TdfGeneratorConfiguration config,
        GenerationInformation generationInformation,
        INamespaceAliasManager namespaceAliasManager)
    {
        _generationInformation = generationInformation;
        _namespaceAliasManager = namespaceAliasManager;
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

        var instantiation = CollectionMap[isArray ? "Array" : CollectionMap.Keys.First(type.Name.StartsWith)];

        _generationInformation.CollectionsGenerated = true;

        return
            instantiation is null
                ? $"[..GetSome(() => {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(genericType, dependencies)})]"
                : instantiation.Generate(parameterInstantiationCodeGenerator.GenerateParameterInstantiation(genericType, dependencies), genericType, _namespaceAliasManager);
    }

    private sealed record Instantiation(InstantiationMethod Method, string code)
    {
        public string Generate(string parameterInstantiation, Type type, INamespaceAliasManager namespaceAliasManager)
        {
            return Method switch
            {
                InstantiationMethod.ToMethod
                    => $"GetSome(() => {parameterInstantiation}).{code}",
                InstantiationMethod.ConstructionMethod
                    => $"{code}<{namespaceAliasManager.GetNamespaceAliasWithDot(type.Namespace)}{type.Name}>([..GetSome(() => {parameterInstantiation})])",
            };
        }
    }

    private enum InstantiationMethod
    {
        ToMethod = 1,
        ConstructionMethod = 2,
    }
}