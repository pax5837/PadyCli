using ProtoToUmlConverter;
using ProtoToUmlConverter.Services;
using TextCopy;

namespace PadyCli.ConsoleApp.Features.ProtoToUmlConverter;

internal class ProtToUmlConverter
{
    private readonly IProtoToUmlConversionService conversionService;

    public ProtToUmlConverter(IProtoToUmlConversionService conversionService)
    {
        this.conversionService = conversionService;
    }

    public int Run(ProtoConverterOptions opts)
    {
        var dir = Directory.GetCurrentDirectory();

        var diagramType = opts.DiagramType switch
        {
            "mermaid" => DiagramProvider.Mermaid,
            "plantuml" => DiagramProvider.PlantUml,
            _ => throw new ArgumentException($"{opts.DiagramType} not supported"),
        };

        var umlLines = conversionService.GenerateUmlFromProto(
            directory: dir,
            targetMessage: opts.EntryPoint,
            diagramProvider: diagramType);

        ClipboardService.SetText(string.Join("\n", umlLines));

        foreach (var line in umlLines)
        {
            Console.WriteLine(line);
        }

        return 0;
    }
}