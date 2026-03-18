namespace PadyCliInteractiveConsoleApp.Features.JsonSanitizer;

internal static class StringSanitizerExtensions
{
    public static string ReplaceWhen(this string input, string oldValue, string newValue, bool predicate)
    {
        return predicate ? input.Replace(oldValue, newValue) : input;
    }
}