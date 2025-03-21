using System.Collections.Immutable;

namespace ProtoToUmlConverter.Services;

public record ProtoFile(string FileName, IImmutableList<string> Lines);