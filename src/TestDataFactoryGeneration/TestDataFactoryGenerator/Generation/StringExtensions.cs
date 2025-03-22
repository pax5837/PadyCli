namespace TestDataFactoryGenerator.Generation;

internal static class StringExtensions
{
    public static string ToCamelCase(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        if (s.Length == 1)
        {
            return s.ToLowerInvariant();
        }

        return char.ToLowerInvariant(s[0]) + s[1..];
    }
}