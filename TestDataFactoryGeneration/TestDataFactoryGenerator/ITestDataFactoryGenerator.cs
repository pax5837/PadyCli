using System.Collections.Immutable;

namespace TestDataFactoryGenerator;

public interface ITestDataFactoryGenerator
{
    public IImmutableList<string> GenerateTestDataFactory(
        string testDataFactoryName,
        string nameSpace,
        bool outputToConsole,
        IImmutableSet<Type> types,
        bool includeHelperClasses);
}