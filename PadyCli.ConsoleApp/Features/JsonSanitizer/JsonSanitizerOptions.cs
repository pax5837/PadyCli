using CommandLine;

namespace PadyCli.ConsoleApp.Features.JsonSanitizer;

[Verb("jsani", HelpText = "Sanitizes json text in the clipboard, and saves the sanitized text in the clipboard.")]
public class JsonSanitizerOptions
{
    [Option('d', "double-curly-braces", Required = false, HelpText = "sanitizes double curly braces in a JSON file.")]
    public bool SanitizeDoubleCurlyBraces { get; set; } = false;
}