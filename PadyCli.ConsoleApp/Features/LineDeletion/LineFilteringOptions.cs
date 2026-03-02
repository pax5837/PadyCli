using CommandLine;

namespace PadyCli.ConsoleApp.Features.LineDeletion;

[Verb("filter-lines", HelpText = "Modifies the content of the clipboard by specifying filter criteria that specify which lines are kept.")]
public class LineFilteringOptions
{
    [Option('c', "contains", Required = false, HelpText = "Keeps lines that contain the specified string. It can not start with '-', a leading '*' will be ignored.")]
    public string? Contains { get; set; }

    [Option('s', "starts-with", Required = false, HelpText = "Keeps lines that start with the specified string. It can not start with '-', a leading '*' will be ignored.")]
    public string? StartsWith { get; set; }

    [Option('r', "regex", Required = false, HelpText = "Keeps lines that match the regex. It can not start with '-', a leading '*' will be ignored.")]
    public string? Regex { get; set; }
}