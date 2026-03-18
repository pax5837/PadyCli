using Spectre.Console;
using TextCopy;

namespace PadyCliInteractiveConsoleApp.Features.JsonSanitizer;

internal class JsonSanitizerService
{
    public OperationResult Run()
    {
        var text = ClipboardService.GetText();
        if (string.IsNullOrWhiteSpace(text))
        {
            AnsiConsole.MarkupLine("[red]No text found in clipboard.[/]");
        }

        var choice = AnsiConsole
            .Prompt(new SelectionPrompt<string>()
                .Title("Do wen need to sanitize double curly braces?")
                .AddChoices("Yes", "No", "Back", "Exit App"));

        if (choice.Equals("Exit App"))
        {
            return OperationResult.ExitApplication;
        }
        
        if (choice.Equals("Back"))
        {
            return OperationResult.Continue;
        }

        var sanitizeDoubleCurlyBraces = choice.Equals("Yes");

        var sanitizedText = text!
            .ReplaceWhen("{{", "{", sanitizeDoubleCurlyBraces)
            .ReplaceWhen("}}", "}", sanitizeDoubleCurlyBraces)
            .Replace("\"{", "{")
            .Replace("}\"", "}")
            .Replace("\\\"", "\"");

        ClipboardService.SetText(sanitizedText);
        AnsiConsole.MarkupLine("[green]The sanitized JSON text has been copied to the clipboard.[/]");

        return OperationResult.Continue;
    }
}