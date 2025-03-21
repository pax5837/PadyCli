using CommandLine;

namespace PadyCli.ConsoleApp.Features.TestClassGeneration;

[Verb("gentestclass", HelpText = "Generates test class")]
public class TestClassGeneratorOptions
{
    [Option('c', "class", Required = true, HelpText = "Class name, case insensitive, can be fully qualified")]
    public string ClassName { get; set; }

    [Option(
        shortName: 'f',
        longName: "compile-on-the-fly",
        HelpText =
            "Whether to use on the fly compilation. This tends to fail more, but makes sure thee targeted project is compiled.",
        Default = false,
        Required = false)]
    public bool UseOnTheFlyCompilation { get; set; }

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Verbose output")]
    public bool Verbose { get; set; }
}