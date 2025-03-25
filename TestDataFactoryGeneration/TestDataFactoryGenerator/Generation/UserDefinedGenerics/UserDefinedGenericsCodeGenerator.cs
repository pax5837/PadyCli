using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.UserDefinedGenerics;

internal class UserDefinedGenericsCodeGenerator : IUserDefinedGenericsCodeGenerator
{
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
        var genericTypeNames = genericTypes.Select((gt, index) => $"T{index + 1}{RemoveGenericPart(gt.Name)}")
            .ToImmutableList();

        return $"{Definitions.GenerationMethodPrefix}{typeName}{string.Join(string.Empty, genericTypeNames)}";
    }

    private string RemoveGenericPart(string typeName)
    {
        return typeName.Split('`', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).First();
    }
}