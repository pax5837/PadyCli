using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.AbstractClasses;

internal class AbstractClassInformationService : IAbstractClassInformationService
{
    public bool IsAbstractClassUsedAsOneOf(Type type, Func<Type, bool> typeNamespacePredicate)
    {
        if (!type.IsAbstract)
        {
            return false;
        }

        return GetDerivedTypes(type, typeNamespacePredicate).Any();
    }

    public IImmutableSet<Type> GetDerivedTypes(Type type, Func<Type, bool> typeNamespacePredicate)
    {
        var derivedTypes = type.Assembly.GetTypes()
            .Where(t => typeNamespacePredicate(t) && t != type && t.IsAssignableTo(type))
            .ToImmutableHashSet();

        return derivedTypes;
    }
}