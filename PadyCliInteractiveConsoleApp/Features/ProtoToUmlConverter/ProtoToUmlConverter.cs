using Microsoft.Extensions.Logging;
using ProtoToUmlConverter;
using ProtoToUmlConverter.Services;
using Spectre.Console;
using TextCopy;

namespace PadyCliInteractiveConsoleApp.Features.ProtoToUmlConverter;

internal class ProtoToUmlConverter
{
    private const string PlantUml = "PlantUML";
    private const string Mermaid = "Mermaid";

    private readonly IProtoToUmlConversionService conversionService;
    private readonly ILogger<ProtoToUmlConverter> logger;

    public ProtoToUmlConverter(
        IProtoToUmlConversionService conversionService,
        ILogger<ProtoToUmlConverter> logger)
    {
        this.conversionService = conversionService;
        this.logger = logger;
    }

    public OperationResult Run()
    {
        try
        {
            var currentDir = Directory.GetCurrentDirectory();
            var dir = AnsiConsole.Ask<string>("Enter folder or confirm current", currentDir);

            var entryPoint = AnsiConsole.Ask<string>("Enter your entrypoint point (service name):");
        
            var diagramTypeChoice = AnsiConsole
                .Prompt(new SelectionPrompt<string>()
                    .Title("Choose diagram type")
                    .AddChoices(PlantUml, Mermaid));

            var diagramType = diagramTypeChoice switch
            {
                Mermaid => DiagramProvider.Mermaid,
                PlantUml => DiagramProvider.PlantUml,
                _ => throw new ArgumentException($"{diagramTypeChoice} not supported"),
            };
        
            var umlLines = conversionService.GenerateUmlFromProto(
                directory: dir,
                targetMessage: entryPoint,
                diagramProvider: diagramType);

            ClipboardService.SetText(string.Join("\n", umlLines));

            foreach (var line in umlLines)
            {
                Console.WriteLine(line);
            }
        
            AnsiConsole.MarkupLine("\n\n[green]The diagram code has been copied to the clipboard.[/]\n\n");

            return OperationResult.Continue;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while generating UML, see inner exception for details");
            return OperationResult.Continue;
        }
    }
}