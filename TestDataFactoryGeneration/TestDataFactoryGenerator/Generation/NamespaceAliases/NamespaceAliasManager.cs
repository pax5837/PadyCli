using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.NamespaceAliases;

internal class NamespaceAliasManager : INamespaceAliasManager
{
    private readonly Dictionary<string, string> _aliasesByNamespace = new();

    public void AddNamespaceForType(Type type)
    {
        GetNamespaceAliasWithDot(type);
    }

    public string GetNamespaceAliasWithDot(Type type)
    {
        if (string.IsNullOrWhiteSpace(type.Namespace) || type.IsInSystemNamespace())
        {
            return string.Empty;
        }

        var ns = GetFullyQualifiedNameSpace(type);
        if (string.IsNullOrEmpty(ns))
        {
            return string.Empty;
        }

        AddNamespaceAlias(ns);

        return _aliasesByNamespace.TryGetValue(ns, out var alias) ? $"{alias}." : string.Empty;
    }

    public IImmutableList<string> GetNamespaceAliasesUsings()
    {
        return _aliasesByNamespace
            .Select(kvp => $"using {kvp.Value} = {kvp.Key};")
            .OrderBy(x => x)
            .ToImmutableList();
    }

    public IImmutableSet<string> GetNamespacesWithAliases()
    {
        return _aliasesByNamespace
            .Keys
            .ToImmutableHashSet();
    }

    private static string? GetFullyQualifiedNameSpace(Type type)
    {
        var ns = type.DeclaringType?.FullName?.Replace("+", ".") ?? type.Namespace;
        return string.IsNullOrEmpty(ns) ? null : ns;
    }

    private void AddNamespaceAlias(string ns)
    {
        if (string.IsNullOrWhiteSpace(ns) || _aliasesByNamespace.ContainsKey(ns))
        {
            return;
        }

        var originalAliasCandidate = GenerateNamespaceAlias(ns);
        var aliasIndex = 0;
        var currentAliasCandiate = originalAliasCandidate;
        while (true)
        {
            if (!_aliasesByNamespace.Values.Contains(currentAliasCandiate))
            {
                _aliasesByNamespace.Add(ns, currentAliasCandiate);
                return;
            }

            aliasIndex++;
            currentAliasCandiate = $"{originalAliasCandidate}{aliasIndex}";

            if (aliasIndex > _aliasesByNamespace.Count)
            {
                var currentAliases = string.Join("\n", _aliasesByNamespace.Select(kvp => $"- {kvp.Key}:{kvp.Value}"));
                throw new InvalidOperationException($"Can not add namespace alias {originalAliasCandidate}, current aliases:\n{currentAliases}");
            }
        }
    }

    private static string GenerateNamespaceAlias(string @namespace)
    {
        return string.Join(string.Empty, @namespace.Split('.').Select(n => n[0]));
    }
}