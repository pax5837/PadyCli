using System.Collections.Immutable;
using System.Linq;

namespace ProtoToUmlConverter.Services.Converter;

internal class Proto2UmlConverter : IProto2UmlConverter
{
    private readonly IProtoParser _protoParser;
    private readonly IUmlGeneratorFactory _umlGeneratorFactory;
    private readonly IFilterService _filterService;

    public Proto2UmlConverter(
        IProtoParser protoParser,
        IUmlGeneratorFactory umlGeneratorFactory,
        IFilterService filterService)
    {
        _protoParser = protoParser;
        _umlGeneratorFactory = umlGeneratorFactory;
        _filterService = filterService;
    }

    public IImmutableList<string> Convert(
        IImmutableSet<ProtoFile> protoFiles,
        DiagramProvider diagramProvider,
        string entryPoint)
    {
        var allProtobufTypes = protoFiles
            .SelectMany(file => _protoParser.Parse(file))
            .ToImmutableHashSet();
        var filteredProtobufTypes = _filterService.FilterByEntryPoint(entryPoint, allProtobufTypes);
        return _umlGeneratorFactory.GetUmlGenerator(diagramProvider).GetCompleteUmlLines(filteredProtobufTypes);
    }
}