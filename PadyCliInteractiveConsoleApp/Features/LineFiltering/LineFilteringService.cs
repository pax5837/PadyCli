using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Spectre.Console;
using TextCopy;

namespace PadyCliInteractiveConsoleApp.Features.LineFiltering;

internal class LineFilteringService
{
    private const string StartsWithFilter = "Starts with filter";
    private const string ContainFilter = "Contains filter";
    private const string RegexFilter = "Regex filter";
    
    public OperationResult Run()
    {
        var lines = ClipboardService.GetText()!.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var filterChoices = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("What filters should be applied")
                .NotRequired()
                .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to confirm)[/]")
                .AddChoices(StartsWithFilter, ContainFilter, RegexFilter));
        
        string? startsWithFilter = filterChoices.Contains(StartsWithFilter)
            ? AnsiConsole.Ask<string>("Enter your [blue]starts with[/] criterion:")
            : null;
        
        string? containsFilter = filterChoices.Contains(ContainFilter)
            ? AnsiConsole.Ask<string>("Enter your [blue]contains[/] criterion:")
            : null;

        string? regexFilter = filterChoices.Contains(RegexFilter)
            ? AnsiConsole.Ask<string>("Enter your [blue]regex[/] criterion:")
            : null;
        
        var filteredLines = lines
            .WhereConditional(
                line => line.StartsWith(startsWithFilter!),
                !string.IsNullOrWhiteSpace(startsWithFilter)!)
            .WhereConditional(
                line => line.Contains(containsFilter!),
                !string.IsNullOrWhiteSpace(containsFilter))
            .WhereConditional(
                line => new Regex(regexFilter!).IsMatch(line),
                !string.IsNullOrWhiteSpace(regexFilter))
            .ToImmutableArray();

        ClipboardService.SetText(string.Join(Environment.NewLine, filteredLines));

        return OperationResult.Continue;
    }
}