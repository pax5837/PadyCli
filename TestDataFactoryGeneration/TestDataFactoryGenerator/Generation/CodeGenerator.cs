using System.Collections.Immutable;
using TestDataFactoryGenerator.Generation.Helpers;

namespace TestDataFactoryGenerator.Generation;

internal class CodeGenerator : ICodeGenerator
{
    private readonly IParameterInstantiationCodeGenerator _parameterInstantiationCodeGenerator;
    private readonly IEitherCodeGenerator _eitherCodeGenerator;
    private readonly ITypeWithConstructorsGenerator _typeWithConstructorsGenerator;
    private readonly IEitherInformationService _eitherInformationService;
    private readonly IHelpersGenerator _helpersGenerator;
    private readonly TdfGeneratorConfiguration _config;

    public CodeGenerator(
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator,
        IEitherCodeGenerator eitherCodeGenerator,
        ITypeWithConstructorsGenerator typeWithConstructorsGenerator,
        IEitherInformationService eitherInformationService,
        IHelpersGenerator helpersGenerator,
        TdfGeneratorConfiguration config)
    {
        _parameterInstantiationCodeGenerator = parameterInstantiationCodeGenerator;
        _eitherCodeGenerator = eitherCodeGenerator;
        _typeWithConstructorsGenerator = typeWithConstructorsGenerator;
        _eitherInformationService = eitherInformationService;
        _helpersGenerator = helpersGenerator;
        _config = config;
    }

    public IImmutableList<string> CreateTestDataFactoryCode(
        string tdfName,
        string nameSpace,
        IImmutableSet<Type> types,
        IImmutableList<string> inputTypeFullNames,
        bool includeHelperClasses)
    {
        var codeDoc = GenerateCodeDoc(inputTypeFullNames);
        var starOfClass = GenerateStartOfClass(tdfName);
        var dependencies = GenerateInitialDependencies();

        var methods = types.OrderBy(x => x.Name).SelectMany(x => CreateGenerationCode(x, dependencies))
            .ToImmutableList();

        var userDefinedMethods = _config.SimpleTypeConfiguration.InstantiationConfigurations
            .SelectMany(CreateMethodsForUserDefinedMethods)
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
                .Concat(userDefinedMethods)
                .RemoveLastWhiteLine()
                .Concat(endOfClass)
                .Concat(_helpersGenerator.GenerateHelpersCode(includeHelperClasses))
                .ToImmutableList();
    }

    private ImmutableList<string> CreateMethodsForUserDefinedMethods(InstantiationConfigurationForType instantiationConfigurationForType)
    {
        if (instantiationConfigurationForType.MethodCodeToAdd.Count == 0)
        {
            return [];
        }

        return instantiationConfigurationForType.MethodCodeToAdd.Select(line => $"{_config.Indent}{line}").Append(string.Empty).ToImmutableList();
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

        foreach (var namespaceToAdd in _config.NamespacesToAdd)
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

    private ImmutableList<string> GenerateStartOfClass(string tdfName)
    {
        return new[]
        {
            $"internal partial class {tdfName}",
            "{",
            $"{_config.Indent}private static readonly IImmutableList<int> {_config.LeadingUnderscore()}zeroBiasedCounts = [0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2];",
            string.Empty,
            $"{_config.Indent}private int GetZeroBiasedCount() => {_config.LeadingUnderscore()}zeroBiasedCounts.OrderBy(_ => {_config.LeadingUnderscore()}random.Next()).First();",
            string.Empty,
            $"{_config.Indent}private readonly Random {_config.LeadingUnderscore()}random;",
            string.Empty,
            $"{_config.Indent}public {tdfName}()",
            $"{_config.Indent}{{",
            $"{_config.Indent}{_config.Indent}{_config.This()}random = new Random();",
            $"{_config.Indent}}}",
            string.Empty,
            $"{_config.Indent}public {tdfName}(int seed)",
            $"{_config.Indent}{{",
            $"{_config.Indent}{_config.Indent}{_config.This()}random = new Random(seed);",
            $"{_config.Indent}}}",
            string.Empty,
            $"{_config.Indent}public {tdfName}(Random random)",
            $"{_config.Indent}{{",
            $"{_config.Indent}{_config.Indent}{_config.This()}random = random;",
            $"{_config.Indent}}}",
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