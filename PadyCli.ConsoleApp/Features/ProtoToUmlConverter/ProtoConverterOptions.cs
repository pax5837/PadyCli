using CommandLine;

namespace PadyCli.ConsoleApp.Features.ProtoToUmlConverter;

[Verb("genprotouml", HelpText = "Converts .proto files to UML diagram.")]
public class ProtoConverterOptions
{
    [Option('e', "entrypoint", Required = true, HelpText = "Defines the entry point to generate the class diagram.")]
    public string EntryPoint { get; set; }

    [Option('t',
        "type",
        Required = false,
        Default = "plantuml",
        HelpText = "chooses the type of diagram 'mermaid' or 'plantuml'.")]
    public string DiagramType { get; set; }
}