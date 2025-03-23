using System.Collections.Immutable;

namespace ProtoToUmlConverter.Services;

public interface IFilterService
{
    IImmutableSet<RawProtoBuffType> FilterByEntryPoint(string entryPointName,
        IImmutableSet<RawProtoBuffType> protoBuffTypes);
}