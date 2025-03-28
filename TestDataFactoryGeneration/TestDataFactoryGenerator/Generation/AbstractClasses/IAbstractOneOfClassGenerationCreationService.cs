using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.AbstractClasses;

internal interface IAbstractOneOfClassGenerationCreationService
{
    IImmutableList<string> CreateGenerationCode(Type type);
}