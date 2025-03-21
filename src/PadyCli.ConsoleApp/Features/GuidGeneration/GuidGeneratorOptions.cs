using CommandLine;

namespace PadyCli.ConsoleApp.Features.GuidGeneration;

[Verb("guids", HelpText = "Generates guids")]
public class GuidGeneratorOptions
{
    [Option('c',
        "count",
        Required = false,
        Default = 3,
        HelpText = "Defines the number of guids generated. Default is 3.")]
    public int count { get; set; }
}