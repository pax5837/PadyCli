using System.Collections.Immutable;
using System.Reflection;

namespace TestDataFactoryGenerator.Generation;

internal static class ParameterInfoExtensions
{
    private static IImmutableSet<string> ReservedKeyWords =
    [
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "while",
    ];
    
    public static string SanitizedName(this ParameterInfo parameter)
    {
        return ReservedKeyWords.Contains(parameter.Name)
            ? $"@{parameter.Name}"
            : parameter.Name;
    }
    
    public static string SanitizedName(this string parameterName)
    {
        return ReservedKeyWords.Contains(parameterName)
            ? $"@{parameterName}"
            : parameterName;
    }
}