using System.Collections.Immutable;

namespace TestDataFactoryGenerator;

public record SimpleTypeConfiguration(string? ParameterNamePlaceholder, ImmutableList<InstantiationConfiguration> InstantiationConfigurations);