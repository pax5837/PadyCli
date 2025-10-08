namespace PadyCli.ConsoleApp.Features.JsonSanitizer;

internal static class stringSanitizerExtensions
{
    public static string ReplaceWhen(this string input, string oldValue, string newValue, bool predicate)
    {
        if(!predicate)
        {
            return input;
        }

        return input.Replace(oldValue, newValue);
    }
}