using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace BuilderGenerator.Sources.CsFilesExplorer;

public class GetCandidateTypeExplorer : IGetCandidateTypeExplorer
{
    public IImmutableList<IGetCandidateTypeExplorer.CandidateType> GetCandidateTypes(IImmutableList<string> csFiles)
    {
        return csFiles.SelectMany(GetNamespacesAndTypeNames).ToImmutableList();
    }

    private IImmutableList<IGetCandidateTypeExplorer.CandidateType> GetNamespacesAndTypeNames(
        string csFile)
    {
        var lines = File.ReadLines(csFile).ToImmutableList();

        var fileNamespace = lines
            .Single(line => line.StartsWith("namespace", StringComparison.Ordinal))
            .Split(new[] { "namespace" }, StringSplitOptions.RemoveEmptyEntries)
            .First()
            .Replace(";", string.Empty)
            .Trim();

        return lines
            .Where(line => line.Contains("class ") || line.Contains("struct ") || line.Contains("record "))
            .Select(line => GetTypeName(line))
            .OfType<string>()
            .Select(name => new IGetCandidateTypeExplorer.CandidateType(fileNamespace, name))
            .ToImmutableList();
    }

    private static string? GetTypeName(string line)
    {
        if (line.Contains("class "))
        {
            return GetTypeName(line, "class");
        }

        if (line.Contains("record "))
        {
            return GetTypeName(line, "record");
        }

        if (line.Contains("struct "))
        {
            return GetTypeName(line, "struct");
        }

        return null;
    }

    private static string GetTypeName(string line, string identifier)
    {
        var indexOfIdentifier = line.IndexOf(identifier);
        return line
            .Substring(indexOfIdentifier + identifier.Length)
            .Split(new[] { ' ', '(', '{' }, StringSplitOptions.RemoveEmptyEntries)
            .First()
            .Trim();
    }
}