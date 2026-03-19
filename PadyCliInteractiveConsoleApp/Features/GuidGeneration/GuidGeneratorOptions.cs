using CommandLine;

namespace PadyCliInteractiveConsoleApp.Features.GuidGeneration;

[Verb("guids", HelpText = "Generates guids")]
public class GuidGeneratorOptions
{
    [Option('c',
        "count",
        Required = false,
        Default = 1,
        HelpText = "Defines the number of guids generated. Default is 1.")]
    public int count { get; set; }
}