using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface INamespaceAliasManager
{
    void AddNamespaceForType(Type type);

    string GetNamespaceAliasWithDot(string? @namespace);

    IImmutableList<string> GetNamespaceAliasesUsings();
}