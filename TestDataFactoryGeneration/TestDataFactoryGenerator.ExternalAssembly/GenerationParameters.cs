using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TypeSelectionWrapper;

public record GenerationParameters(
    string TestDataFactoryName,
    string NameSpace,
    IImmutableSet<string> TypeNames,
    bool OutputToConsole,
    string? WorkingDirectory = null);