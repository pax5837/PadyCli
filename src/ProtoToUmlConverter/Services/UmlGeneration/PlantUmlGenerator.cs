using System;
using System.Collections.Immutable;
using System.Linq;

namespace ProtoToUmlConverter.Services.UmlGeneration;

public class PlantUmlGenerator : IUmlGenerator
{
    public IImmutableList<string> GetCompleteUmlLines(IImmutableSet<RawProtoBuffType> protoBuffTypes)
    {
        return protoBuffTypes
            .SelectMany(GenerateClassDoc)
            .Prepend("@startuml")
            .Append("@enduml")
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
            .Select(d => GenerateDependencyArrow(rawProtoBuffType, d))
            .ToImmutableList();
    }

    private static string GenerateDependencyArrow(
        RawProtoBuffType rawProtoBuffType,
        RawDependency d)
    {
        var multiplicityText = d.IsRepeated
            ? "\"*\" "
            : string.Empty;

        return $"{rawProtoBuffType.Name} -- {multiplicityText}{d.TypeName}";
    }

    private IImmutableList<string> GenerateClass(RawProtoBuffType rawProtoBuffType)
    {
        var kind = rawProtoBuffType.Kind switch
        {
            Kind.Message => "class",
            Kind.Enumeration => "enum",
            _ => throw new InvalidOperationException($"{rawProtoBuffType.Kind} not supported"),
        };

        return rawProtoBuffType.DependenciesWithNameSpace
            .Select(GenerateField)
            .Union(rawProtoBuffType.EnumValues.Select(GenerateEnumValue))
            .Prepend($"{kind} {rawProtoBuffType.Name} {{")
            .Append("}")
            .ToImmutableList();
    }

    private static string GenerateField(RawDependency d)
    {
        var isRepeated = d.IsRepeated ? "[]" : "";
        return $"   {{field}}{d.FieldName} : {d.TypeName}{isRepeated}";
    }

    private static string GenerateEnumValue(string enumValue)
    {
        return $"   {enumValue}";
    }
}