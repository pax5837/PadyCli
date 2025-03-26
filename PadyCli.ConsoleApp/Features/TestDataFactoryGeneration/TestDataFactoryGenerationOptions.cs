using CommandLine;

namespace PadyCli.ConsoleApp.Features.TestDataFactoryGeneration;

[Verb("gen-tdf", HelpText = "Generates test data factory")]
public class TestDataFactoryGenerationOptions
{
    [Option('c', "classes", Required = true, HelpText = "Comma separated class names")]
    public string CommaSepparatedClassNames { get; set; }

    [Option('n', "namespace", Required = true, HelpText = "Namespace of the test data factory")]
    public string TestDataFactoryNamespace { get; set; }

    [Option('t', "tdf-name", Required = true, HelpText = "Class name of the test data factory")]
    public string TestDataFactoryClassName { get; set; }

    [Option('o', "include-optionals", Required = true, Default = false, HelpText = "Include code for optional parameters")]
    public bool IncludeOptionals { get; set; }

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Verbose output")]
    public bool Verbose { get; set; }
}