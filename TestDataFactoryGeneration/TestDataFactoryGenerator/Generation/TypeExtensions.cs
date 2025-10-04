namespace TestDataFactoryGenerator.Generation;

internal static class TypeExtensions
{
    public static bool IsNotInSystemNamespace(this Type type)
    {
        return
            type.Namespace != null &&
            !type.Namespace.StartsWith("System", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsInSystemNamespace(this Type type)
    {
        return
            type.Namespace != null &&
            type.Namespace.StartsWith("System", StringComparison.OrdinalIgnoreCase);
    }
}