using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal interface ICodeGenerator
{
    IImmutableList<string> CreateTestDataFactoryCode(string tdfName,
        string nameSpace,
        IImmutableSet<Type> types,
        IImmutableList<string> inputTypeFullNames, bool includeOptionalsCode);

    IImmutableList<string> CreateGenerationCode(Type t, HashSet<string> dependencies);
}