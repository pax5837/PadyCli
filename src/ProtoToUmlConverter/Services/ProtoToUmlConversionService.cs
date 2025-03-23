using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace ProtoToUmlConverter.Services;

internal class ProtoToUmlConversionService : IProtoToUmlConversionService
{
    private readonly IProto2UmlConverter _converter;

    public ProtoToUmlConversionService(IProto2UmlConverter converter)
    {
        _converter = converter;
    }

    public IImmutableList<string> GenerateUmlFromProto(
        string directory,
        string targetMessage,
        DiagramProvider diagramProvider)
    {
        var allProtoFiles = Directory
            .GetFiles(directory, "*.proto", SearchOption.AllDirectories)
            .Select(f => new ProtoFile(f, File.ReadAllLines(f).ToImmutableList()))
            .ToImmutableHashSet();

        return _converter.Convert(
            protoFiles: allProtoFiles,
            diagramProvider: diagramProvider,
            entryPoint: targetMessage.Replace(".proto", string.Empty));
    }
}