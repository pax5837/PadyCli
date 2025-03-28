using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface IAbstractOneOfClassGenerationCreationService
{
    IImmutableList<string> CreateGenerationCode(Type type);
}