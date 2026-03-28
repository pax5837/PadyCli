namespace ProtoEditorConsoleApp;

/// <summary>
/// Lightweight proto definition locator.
/// Searches .proto files for message/enum definitions by name,
/// without depending on the internal ProtoToUmlConverter parser.
/// </summary>
internal static class ProtoDefinitionFinder
{
    /// <summary>
    /// Search all .proto files under <paramref name="rootDirectory"/> for a
    /// message or enum definition matching <paramref name="typeName"/>.
    /// Returns the first match as a <see cref="ProtoDefinitionLocation"/>,
    /// or null if nothing is found.
    /// </summary>
    public static ProtoDefinitionLocation? Find(string rootDirectory, string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        // Strip namespace prefix if the user is sitting on "pkg.TypeName"
        var simpleName = typeName.Contains('.')
            ? typeName[(typeName.LastIndexOf('.') + 1)..]
            : typeName;

        foreach (var filePath in EnumerateProtoFiles(rootDirectory))
        {
            var (found, lineIndex) = SearchFile(filePath, simpleName);
            if (found)
                return new ProtoDefinitionLocation(filePath, lineIndex);
        }

        return null;
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static IEnumerable<string> EnumerateProtoFiles(string root)
    {
        try
        {
            return Directory.EnumerateFiles(root, "*.proto", SearchOption.AllDirectories)
                .OrderBy(f => f);
        }
        catch
        {
            return [];
        }
    }

    private static (bool Found, int LineIndex) SearchFile(string filePath, string typeName)
    {
        string[] lines;
        try { lines = File.ReadAllLines(filePath); }
        catch { return (false, 0); }

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].TrimStart();
            if (IsDefinitionLine(trimmed, typeName))
                return (true, i);
        }

        return (false, 0);
    }

    private static bool IsDefinitionLine(string trimmedLine, string typeName)
    {
        return trimmedLine.StartsWith($"message {typeName}", StringComparison.Ordinal)
               || trimmedLine.StartsWith($"enum {typeName}", StringComparison.Ordinal);
    }
}

internal record ProtoDefinitionLocation(string FilePath, int LineIndex);

