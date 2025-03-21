using System.Collections.Immutable;

namespace ProtoToUmlConverter.Services.Proto;

public class FilterService : IFilterService
{
    public IImmutableSet<RawProtoBuffType> FilterByEntryPoint(
        string entryPointName,
        IImmutableSet<RawProtoBuffType> protoBuffTypes)
    {
        var entryProto = protoBuffTypes.Single(pbt => pbt.Name == entryPointName);

        var includedTypes = new HashSet<RawProtoBuffType>();
        PopulateIncludedTypes(entryProto, includedTypes, protoBuffTypes);
        return includedTypes.ToImmutableHashSet();
    }

    private void PopulateIncludedTypes(RawProtoBuffType protobufType, HashSet<RawProtoBuffType> includedTypes,
        IImmutableSet<RawProtoBuffType> allTypes)
    {
        if (!includedTypes.Any(it => it.Name == protobufType.Name && it.Namespace == protobufType.Namespace))
        {
            includedTypes.Add(protobufType);
        }

        foreach (var dependency in protobufType.DependenciesWithNameSpace)
        {
            if (includedTypes.Any(it => it.Name == dependency.TypeName && it.Namespace == dependency.Namespace))
            {
                continue;
            }

            try
            {
                var dep = allTypes.SingleOrDefault(it =>
                    it.Name == dependency.TypeName && it.Namespace == dependency.Namespace);
                if (dep != null)
                {
                    includedTypes.Add(dep);
                    PopulateIncludedTypes(dep, includedTypes, allTypes);
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"Problem when getting {dependency.TypeName}");
                throw;
            }
        }
    }
}