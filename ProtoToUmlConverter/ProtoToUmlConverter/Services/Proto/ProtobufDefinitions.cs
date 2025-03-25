using System.Collections.Generic;
using System.Collections.Immutable;

namespace ProtoToUmlConverter.Services.Proto;

internal static class ProtobufDefinitions
{
    public static IImmutableSet<string> Keywords = new HashSet<string>
    {
        "syntax",
        "option",
    }.ToImmutableHashSet();

    public static IImmutableSet<string> BaseTypes = new HashSet<string>
    {
        "string",
        "int32",
        "float",
        "int64",
        "uint32",
        "uint64",
        "sint32",
        "sint64",
        "fixed32",
        "fixed64",
        "sfixed32",
        "sfixed64",
        "bool",
        "bytes",
    }.ToImmutableHashSet();

    public static IImmutableSet<string> FieldModifiers = new HashSet<string>
    {
        "optional",
        FieldModifierRepeated,
    }.ToImmutableHashSet();

    public const string FieldModifierRepeated = "repeated";

    public const string FieldModifierOneOf = "oneof";
}