using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.FactoryGeneration;

internal interface ITypeLister
{
    IImmutableSet<Type> GetAllTypes(IImmutableSet<Type> inputTypes);
}