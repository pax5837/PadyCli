using Spectre.Console;
using TextCopy;

namespace PadyCliInteractiveConsoleApp.Features.GuidGeneration;

internal class GuidGenerator
{
    public OperationResult GenerateOneGuid()
    {
        var guid = Guid.NewGuid();
        AnsiConsole.MarkupLine($"Your generated GUID is '[green]{guid.ToString()}[/]'. It has been copied to the clipboard.");
        ClipboardService.SetText(guid.ToString());
        return OperationResult.Continue;
    }
}