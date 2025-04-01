using System.Collections.Immutable;

namespace ProtoToUmlConverter.Services;

public record RawProtoBuffType(
    string Name,
    string Namespace,
    Kind Kind,
    IImmutableSet<RawDependency> DependenciesWithNameSpace,
    IImmutableList<string> EnumValues);

public enum Kind
{
    Message,
    Enumeration,
}

public record RawDependency(string Namespace, string TypeName, string FieldName, bool IsRepeated, bool IsOptional);

public interface IProtoParser
{
    IImmutableList<RawProtoBuffType> Parse(ProtoFile protoFile);
}