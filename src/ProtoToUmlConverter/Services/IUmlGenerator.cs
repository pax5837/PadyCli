using System.Collections.Immutable;

namespace ProtoToUmlConverter.Services;

public interface IUmlGenerator
{
    public IImmutableList<string> GetCompleteUmlLines(IImmutableSet<RawProtoBuffType> protoBuffTypes);
}