using System.Collections.Immutable;
using System.Text.RegularExpressions;
using TextCopy;

namespace PadyCli.ConsoleApp.Features.LineDeletion;

internal class LineFilterer
{
    public int Run(LineFilteringOptions options)
    {
        var lines = ClipboardService.GetText()!.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        if (options.StartsWith is not null && options.StartsWith.StartsWith('*'))
        {
            options.StartsWith = options.StartsWith[1..];
        }

        if (options.Contains is not null && options.Contains.StartsWith('*'))
        {
            options.Contains = options.Contains[1..];
        }

        if (options.Regex is not null && options.Regex.StartsWith('*'))
        {
            options.Contains = options.Regex[1..];
        }


        var regex = options.Regex is not null ? new Regex(options.Regex) : null;
        var filteredLines = lines
            .WhereConditional(
                line => line.Contains(options.Contains!),
                options.Contains is not null)
            .WhereConditional(
                line => line.StartsWith(options.StartsWith!),
                options.StartsWith is not null)
            .WhereConditional(
                line => regex!.IsMatch(line),
                options.Regex is not null)
            .ToImmutableArray();

        ClipboardService.SetText(string.Join(Environment.NewLine, filteredLines));

        return 0;
    }
}