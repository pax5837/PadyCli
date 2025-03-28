using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface ICodeGenerator
{
    IImmutableList<string> CreateTestDataFactoryCode(string tdfName,
        string nameSpace,
        IImmutableSet<Type> types,
        IImmutableList<string> inputTypeFullNames, bool includeHelperClasses);

    IImmutableList<string> CreateGenerationMethod(Type type, HashSet<string> dependencies);
}