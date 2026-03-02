namespace PadyCli.ConsoleApp.Features.LineDeletion;

internal static class LinesExtensions
{
    public static IEnumerable<string> WhereConditional(
        this IEnumerable<string> lines,
        Func<string, bool> linePredicate,
        bool doApplyPredicate)
    {
        return doApplyPredicate
            ? lines.Where(linePredicate)
            : lines;
    }
}