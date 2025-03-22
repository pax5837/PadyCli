using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

public interface IConfiguration
{
    IImmutableSet<string> NamespacesToAdd { get; }

    string? EitherNamespace { get; }

    IImmutableDictionary<string, CustomInstantiationInfo>  CustomInstantiationInfoByFullName { get; }
}

public record CustomInstantiationInfo(string TypeFullName, string InstantiationCode, string NamespaceToAdd);