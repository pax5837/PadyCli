using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Optionals;

internal interface IOptionalsGenerator
{
    IImmutableList<string> GenerateOptionalsCode(bool includeOptionalsCode);
}