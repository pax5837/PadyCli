using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface INamespaceAliasManager
{
    void AddNamespaceForType(Type type);

    string GetNamespaceAliasWithDot(Type type);

    IImmutableList<string> GetNamespaceAliasesUsings();

    IImmutableSet<string> GetNamespacesWithAliases();
}