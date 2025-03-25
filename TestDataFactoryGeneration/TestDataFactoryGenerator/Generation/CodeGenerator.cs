using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal class CodeGenerator : ICodeGenerator
{
    private readonly IParameterInstantiationCodeGenerator _parameterInstantiationCodeGenerator;
    private readonly IEitherCodeGenerator _eitherCodeGenerator;
    private readonly ITypeWithConstructorsGenerator _typeWithConstructorsGenerator;
    private readonly IEitherInformationService _eitherInformationService;
    private readonly TdfGeneratorConfiguration _tdfGeneratorConfiguration;

    public CodeGenerator(
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator,
        IEitherCodeGenerator eitherCodeGenerator,
        ITypeWithConstructorsGenerator typeWithConstructorsGenerator,
        IEitherInformationService eitherInformationService,
        TdfGeneratorConfiguration tdfGeneratorConfiguration)
    {
        _parameterInstantiationCodeGenerator = parameterInstantiationCodeGenerator;
        _eitherCodeGenerator = eitherCodeGenerator;
        _typeWithConstructorsGenerator = typeWithConstructorsGenerator;
        _eitherInformationService = eitherInformationService;
        _tdfGeneratorConfiguration = tdfGeneratorConfiguration;
    }

    public IImmutableList<string> CreateTestDataFactoryCode(
        string tdfName,
        string nameSpace,
        IImmutableSet<Type> types,
        IImmutableList<string> inputTypeFullNames)
    {
        var codeDoc = GenerateCodeDoc(inputTypeFullNames);
        var starOfClass = GenerateStartOfClass(tdfName);
        var dependencies = GenerateInitialDependencies();

        var methods = types.OrderBy(x => x.Name).SelectMany(x => CreateGenerationCode(x, dependencies))
            .ToImmutableList();

        var systemUsings = dependencies
            .Where(x => x.StartsWith("System"))
            .OrderBy(x => x)
            .Select(d => $"using {d};").ToImmutableList();

        var usings = dependencies
            .Where(x => !x.StartsWith("System"))
            .Where(x => !IsSubsetOfNamespace(x, nameSpace))
            .OrderBy(x => x)
            .Select(d => $"using {d};").ToImmutableList();

        var endOfClass = new[] { "}" }.ToImmutableList();

        return
            systemUsings
                .Append(string.Empty)
                .Concat(usings)
                .Append(string.Empty)
                .Append($"namespace {nameSpace};")
                .Append(string.Empty)
                .Concat(codeDoc)
                .Concat(starOfClass)
                .Concat(methods)
                .RemoveLastWhiteLine()
                .Concat(endOfClass)
                .ToImmutableList();
    }

    private static bool IsSubsetOfNamespace(string subsetCandidate, string nameSpace)
    {
        var splitNamespace = nameSpace.Split(".");
        var splitSubsetCandidate = subsetCandidate.Split(".");
        if (splitNamespace.Length < splitSubsetCandidate.Length)
        {
            return false;
        }

        return splitNamespace.Take(splitSubsetCandidate.Length).SequenceEqual(splitSubsetCandidate);
    }

    private HashSet<string> GenerateInitialDependencies()
    {
        var dependencies = new HashSet<string>();

        foreach (var namespaceToAdd in _tdfGeneratorConfiguration.NamespacesToAdd)
        {
            dependencies.Add(namespaceToAdd);
        }

        return dependencies;
    }

    private static ImmutableList<string> GenerateCodeDoc(IImmutableList<string> inputTypeFullNames)
    {
        var typeList = inputTypeFullNames.Select(t => $"/// - <see cref=\"{t}\"/><br/>").ToImmutableList();

        return typeList
            .Prepend(
                "/// This class was auto generated using TestDataFactoryGenerator, for following types:<br/>")
            .Prepend("/// <summary>")
            .Append(
                "/// This class can be edited, but it is preferable to not touch this file, and extend the partial class in a separate file.")
            .Append("/// </summary>")
            .ToImmutableList();
    }

    private static ImmutableList<string> GenerateStartOfClass(string tdfName)
    {
        return new[]
        {
            $"internal partial class {tdfName}",
            "{",
            "\tprivate readonly Random _random;",
            string.Empty,
            $"\tpublic {tdfName}()",
            "\t{",
            "\t\t_random = new Random();",
            "\t}",
            string.Empty,
            $"\tpublic {tdfName}(int seed)",
            "\t{",
            "\t\t_random = new Random(seed);",
            "\t}",
            string.Empty,
            $"\tpublic {tdfName}(Random random)",
            "\t{",
            "\t\t_random = random;",
            "\t}",
            string.Empty,
        }.ToImmutableList();
    }

    public IImmutableList<string> CreateGenerationCode(Type t, HashSet<string> dependencies)
    {
        if (t.IsEnum)
        {
            return ImmutableList<string>.Empty;
        }

        if (_eitherInformationService.IsEither(t))
        {
            return _eitherCodeGenerator.CreateGenerationCodeForEither(
                t,
                dependencies,
                _parameterInstantiationCodeGenerator);
        }

        return _typeWithConstructorsGenerator.CreateGenerationCode(
            t,
            dependencies);
    }
}