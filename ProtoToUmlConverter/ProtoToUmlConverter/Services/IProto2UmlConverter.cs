using System.Collections.Immutable;

namespace ProtoToUmlConverter.Services;

public interface IProto2UmlConverter
{
    public IImmutableList<string> Convert(IImmutableSet<ProtoFile> protoFiles, DiagramProvider diagramProvider,
        string entryPoint);
}