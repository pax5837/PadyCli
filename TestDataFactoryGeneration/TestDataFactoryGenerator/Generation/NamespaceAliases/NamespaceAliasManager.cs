using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.NamespaceAliases;

internal class NamespaceAliasManager : INamespaceAliasManager
{
    private readonly Dictionary<string, string> _aliasesByNamespace = new();

    public void AddNamespaceForType(Type type)
    {
        if (type.IsInSystemNamespace())
        {
            return;
        }

        var ns = type.Namespace ?? string.Empty;

        if (_aliasesByNamespace.ContainsKey(ns))
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

    public string GetNamespaceAliasWithDot(string? @namespace)
    {
        if (@namespace == null)
        {
            return string.Empty;
        }

        return _aliasesByNamespace.TryGetValue(@namespace, out var alias) ? $"{alias}." : string.Empty;
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

    private static string GenerateNamespaceAlias(string @namespace)
    {
        return string.Join(string.Empty, @namespace.Split('.').Select(n => n[0]));
    }
}