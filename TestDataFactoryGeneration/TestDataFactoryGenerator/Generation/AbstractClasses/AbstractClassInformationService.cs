namespace TestDataFactoryGenerator.Generation.AbstractClasses;

internal class AbstractClassInformationService : IAbstractClassInformationService
{
    public bool IsAbstractClassUsedAsOneOf(Type t)
    {
        return t.IsAbstract;
    }
}