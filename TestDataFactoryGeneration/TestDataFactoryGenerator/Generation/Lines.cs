using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal class Lines
{
    private readonly TdfGeneratorConfiguration _config;
    private readonly  List<string> _lines = new List<string>();

    public Lines(TdfGeneratorConfiguration config)
    {
        _config = config;
    }

    public Lines Add(string line)
    {
        _lines.Add(line);
        return this;
    }

    public Lines AddEmptyLine()
    {
        _lines.Add(string.Empty);
        return this;
    }

    public Lines AddRange(IImmutableList<(ushort Indents, string Line)> lines)
    {
        foreach (var (indents, line) in lines)
        {
            Add(indents, line);
        }
        return this;
    }

    public Lines Add(ushort indents, string line)
    {
        _lines.Add($"{_config.Indents(indents)}{line}");
        return this;
    }

    public IImmutableList<string> ToImmutableList()
    {
        return _lines.ToImmutableList();
    }
}