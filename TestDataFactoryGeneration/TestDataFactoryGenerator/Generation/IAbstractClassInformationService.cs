using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface IAbstractClassInformationService
{
    bool IsAbstractClassUsedAsOneOf(Type type, Func<Type, bool> typeNamespacePredicate);

    IImmutableSet<Type> GetDerivedTypes(Type type, Func<Type, bool> typeNamespacePredicate);
}