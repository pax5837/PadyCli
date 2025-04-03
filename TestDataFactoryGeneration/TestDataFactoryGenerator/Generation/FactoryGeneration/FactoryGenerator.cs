using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace TestDataFactoryGenerator.Generation.FactoryGeneration;

internal class FactoryGenerator : ITestDataFactoryGenerator
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly ITypeLister _typeLister;
    private readonly ILogger<FactoryGenerator> _logger;

    public FactoryGenerator(
        ICodeGenerator codeGenerator,
        ITypeLister typeLister,
        ILogger<FactoryGenerator> logger)
    {
        _codeGenerator = codeGenerator;
        _typeLister = typeLister;
        _logger = logger;
    }

    public IImmutableList<string> GenerateTestDataFactory(
        string testDataFactoryName,
        string nameSpace,
        bool outputToConsole,
        IImmutableSet<Type> types,
        bool includeHelperClasses)
    {
        try
        {
            var allTypes = _typeLister.GetAllTypes(types.ToImmutableHashSet());
            var inputTypeFullNames = types.Select(t => t.FullName).OfType<string>().ToImmutableList();

            var lines = _codeGenerator.CreateTestDataFactoryCode(
                tdfName: testDataFactoryName,
                nameSpace: nameSpace,
                types: allTypes,
                inputTypeFullNames: inputTypeFullNames,
                includeHelperClasses: includeHelperClasses);

            if (outputToConsole)
            {
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
            }

            return lines;
        }
        catch (ReflectionTypeLoadException e)
        {
            const string errorMessage = "An exception of type `ReflectionTypeLoadException` was  thrown."
                                        + "\nThis error could be cause by a type located in a dll from a nuget package, you could:"
                                        + "\n- use <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies> in your csproj"
                                        + "\n- run a `dotnet publish` or even `dotnet publish --self-contained true`, and pick the dll in the publish directory."
                                        + "\n\n\n";

            _logger.LogError(e, errorMessage);
        }

        return [string.Empty];
    }
}