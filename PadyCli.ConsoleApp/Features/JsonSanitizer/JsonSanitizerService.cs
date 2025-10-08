using TextCopy;

namespace PadyCli.ConsoleApp.Features.JsonSanitizer;

internal class JsonSanitizerService
{
    public int Run(JsonSanitizerOptions options)
    {
        var text = ClipboardService.GetText();
        var sanitizedText = text
                .ReplaceWhen("{{", "{", options.SanitizeDoubleCurlyBraces)
                .ReplaceWhen("}}", "}", options.SanitizeDoubleCurlyBraces)
                .Replace("\"{", "{")
                .Replace("}\"", "}")
                .Replace("\\\"", "\"")
            ;

        ClipboardService.SetText(sanitizedText);

        return 0;
    }
}