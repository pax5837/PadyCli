using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.UserDefinedGenerics;

internal class UserDefinedGenericsCodeGenerator : IUserDefinedGenericsCodeGenerator
{
    private readonly INamespaceAliasManager _namespaceAliasManager;

    public UserDefinedGenericsCodeGenerator(INamespaceAliasManager namespaceAliasManager)
    {
        _namespaceAliasManager = namespaceAliasManager;
    }

    public bool IsAUserDefinedGenericType(Type type)
    {
        return type.IsGenericType && !type.Namespace!.StartsWith("System");
    }

    public string GenerateInstantiationCode(Type type)
    {
        return $"{MethodName(type)}()";
    }

    public string MethodName(Type type)
    {
        var genericTypes = type.GenericTypeArguments;
        var typeName = RemoveGenericPart(type.Name);
        _namespaceAliasManager.AddNamespaceForType(type);
        var genericTypeNames = genericTypes.Select((gt, index) => $"T{index + 1}{RemoveGenericPart(gt.Name)}")
            .ToImmutableList();

        return $"{Definitions.GenerationMethodPrefix}{typeName}{string.Join(string.Empty, genericTypeNames)}";
    }

    private string RemoveGenericPart(string typeName)
    {
        return typeName.Split('`', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).First();
    }
}