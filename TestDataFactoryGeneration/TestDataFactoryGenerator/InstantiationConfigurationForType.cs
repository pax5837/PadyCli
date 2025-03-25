using System.Collections.Immutable;

namespace TestDataFactoryGenerator;

public record InstantiationConfigurationForType(Type Type, string InstantiationCode, string? NamespaceToAdd, IImmutableList<string> MethodCodeToAdd);