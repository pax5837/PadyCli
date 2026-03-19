using Spectre.Console;
using TextCopy;

namespace PadyCliInteractiveConsoleApp.Features.GuidGeneration;

internal class GuidGenerator
{
    public OperationResult GenerateGuidInteractive()
    {
        var guid = Guid.NewGuid();
        AnsiConsole.MarkupLine($"Your generated GUID is '[green]{guid.ToString()}[/]'. It has been copied to the clipboard.");
        ClipboardService.SetText(guid.ToString());
        return OperationResult.Continue;
    }
    
    public int Run(GuidGeneratorOptions options)
    {
        var guids = GetGuidList(options);
        guids.ForEach(g => Console.WriteLine(g.Guid));
        foreach (var guid in guids)
        {
            ClipboardService.SetText(guid.Guid.ToString());
            if (guid.Index < options.count)
            {
                Task.Delay(1000).Wait();
            }
        }

        return 0;
    }
    
    private static List<(int Index, Guid Guid)> GetGuidList(GuidGeneratorOptions options)
    {
        return Enumerable.Range(1, options.count).Select(i => (i, Guid.NewGuid())).ToList();
    }
}