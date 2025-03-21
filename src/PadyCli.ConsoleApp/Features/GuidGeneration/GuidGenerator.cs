using TextCopy;

namespace PadyCli.ConsoleApp.Features.GuidGeneration;

internal class GuidGenerator
{
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