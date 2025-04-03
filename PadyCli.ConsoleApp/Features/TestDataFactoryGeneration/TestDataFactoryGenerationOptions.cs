using CommandLine;

namespace PadyCli.ConsoleApp.Features.TestDataFactoryGeneration;

[Verb("gen-tdf", HelpText = "Generates test data factory")]
public class TestDataFactoryGenerationOptions
{
    [Option('c', "classes", Required = true, HelpText = "Comma separated class names")]
    public string CommaSepparatedClassNames { get; set; }

    [Option('n', "namespace", Required = false, Default = "Namespace", HelpText = "Namespace of the test data factory")]
    public string TestDataFactoryNamespace { get; set; }

    [Option('t', "tdf-name", Required = false, Default ="Tdf",  HelpText = "Class name of the test data factory")]
    public string TestDataFactoryClassName { get; set; }

    [Option('h', "include-helpers", Required = false, Default = false, HelpText = "Include helper classes used by the generated test data factory")]
    public bool IncludeOptionals { get; set; }

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Verbose output")]
    public bool Verbose { get; set; }
}