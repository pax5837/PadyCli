using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface ITypeLister
{
    IImmutableSet<Type> GetAllTypes(IImmutableSet<Type> inputTypes);
}