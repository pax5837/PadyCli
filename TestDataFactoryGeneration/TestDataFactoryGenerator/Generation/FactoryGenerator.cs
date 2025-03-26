using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal class FactoryGenerator : ITestDataFactoryGenerator
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly ITypeLister _typeLister;

    public FactoryGenerator(
        ICodeGenerator codeGenerator,
        ITypeLister typeLister)
    {
        _codeGenerator = codeGenerator;
        _typeLister = typeLister;
    }

    public IImmutableList<string> GenerateTestDataFactory(
        string testDataFactoryName,
        string nameSpace,
        bool outputToConsole,
        IImmutableSet<Type> types,
        bool includeOptionalsCode)
    {
        var allTypes = _typeLister.GetAllTypes(types.ToImmutableHashSet());
        var inputTypeFullNames = types.Select(t => t.FullName).OfType<string>().ToImmutableList();

        var lines = _codeGenerator.CreateTestDataFactoryCode(
            testDataFactoryName,
            nameSpace,
            allTypes,
            inputTypeFullNames,
            includeOptionalsCode: includeOptionalsCode);

        if (outputToConsole)
        {
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        return lines;
    }
}