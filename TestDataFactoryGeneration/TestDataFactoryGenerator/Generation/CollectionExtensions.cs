namespace TestDataFactoryGenerator.Generation;

internal static class CollectionExtensions
{
    public static IEnumerable<string> RemoveLastWhiteLine(this IEnumerable<string> collection)
    {
        var list = collection.ToList();

        if (string.IsNullOrWhiteSpace(list.Last()))
        {
            return list.Take(list.Count - 1);
        }

        return list;
    }

    public static void AddConditional(this List<string> collection, string element, bool predicate)
    {
        if (predicate)
        {
            collection.Add(element);
        }
    }
}