using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ProtoToUmlConverter.Services.UmlGeneration;

internal class MermaidGenerator : IUmlGenerator
{
    public IImmutableList<string> GetCompleteUmlLines(IImmutableSet<RawProtoBuffType> protoBuffTypes)
    {
        return protoBuffTypes
            .SelectMany(GenerateClassDoc)
            .Prepend("classDiagram")
            .ToImmutableList();
    }

    private IImmutableList<string> GenerateClassDoc(RawProtoBuffType rawProtoBuffType)
    {
        return GenerateClass(rawProtoBuffType)
            .Union(GenerateDependencies(rawProtoBuffType))
            .ToImmutableList();
    }

    private IImmutableList<string> GenerateDependencies(RawProtoBuffType rawProtoBuffType)
    {
        return rawProtoBuffType.DependenciesWithNameSpace
            .Where(d => !string.IsNullOrWhiteSpace(d.Namespace))
            .Select(d => $"{rawProtoBuffType.Name} --> {d.TypeName}")
            .ToImmutableList();
    }

    private IImmutableList<string> GenerateClass(RawProtoBuffType rawProtoBuffType)
    {
        return rawProtoBuffType.DependenciesWithNameSpace
            .Select(d => GenerateField(d, rawProtoBuffType))
            .Union(GenerateEnumValues(rawProtoBuffType))
            .Prepend($"class {rawProtoBuffType.Name} {{")
            .Append("}")
            .ToImmutableList();
    }

    private static string GenerateField(RawDependency d, RawProtoBuffType rawProtoBuffType)
    {
        var repeatedChar = d.IsRepeated ? "[]" : string.Empty;
        var optionalChar = d.IsOptional ? "?" : string.Empty;
        return $"   {d.TypeName}{repeatedChar}{optionalChar} {d.FieldName}";
    }

    private static IEnumerable<string> GenerateEnumValues(RawProtoBuffType rawProtoBuffType)
    {
        if (rawProtoBuffType.EnumValues.Count < 1)
        {
            return [];
        }

        return rawProtoBuffType.EnumValues.Select(d => $"   {d}").Prepend("   <<enumeration>>");
    }
}