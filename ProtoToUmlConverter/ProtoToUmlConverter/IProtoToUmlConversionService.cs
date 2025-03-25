using System.Collections.Immutable;
using ProtoToUmlConverter.Services;

namespace ProtoToUmlConverter;

public interface IProtoToUmlConversionService
{
    IImmutableList<string> GenerateUmlFromProto(
        string directory,
        string targetMessage,
        DiagramProvider diagramProvider);
}