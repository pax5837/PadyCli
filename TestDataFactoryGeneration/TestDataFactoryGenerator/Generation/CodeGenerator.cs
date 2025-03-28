using System.Collections.Immutable;
using TestDataFactoryGenerator.Generation.AbstractClasses;
using TestDataFactoryGenerator.Generation.Helpers;

namespace TestDataFactoryGenerator.Generation;

internal class CodeGenerator : ICodeGenerator
{
    private readonly IParameterInstantiationCodeGenerator _parameterInstantiationCodeGenerator;
    private readonly IEitherCodeGenerator _eitherCodeGenerator;
    private readonly ITypeWithConstructorsGenerator _typeWithConstructorsGenerator;
    private readonly IEitherInformationService _eitherInformationService;
    private readonly IHelpersGenerator _helpersGenerator;
    private readonly IRandomizerCallerGenerator _randomizerCallerGenerator;
    private readonly IAbstractClassInformationService _abstractClassInformationService;
    private readonly TdfGeneratorConfiguration _config;

    public CodeGenerator(
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator,
        IEitherCodeGenerator eitherCodeGenerator,
        ITypeWithConstructorsGenerator typeWithConstructorsGenerator,
        IEitherInformationService eitherInformationService,
        IHelpersGenerator helpersGenerator,
        IRandomizerCallerGenerator randomizerCallerGenerator,
        IAbstractClassInformationService abstractClassInformationService,
        TdfGeneratorConfiguration config)
    {
        _parameterInstantiationCodeGenerator = parameterInstantiationCodeGenerator;
        _eitherCodeGenerator = eitherCodeGenerator;
        _typeWithConstructorsGenerator = typeWithConstructorsGenerator;
        _eitherInformationService = eitherInformationService;
        _helpersGenerator = helpersGenerator;
        _randomizerCallerGenerator = randomizerCallerGenerator;
        _abstractClassInformationService = abstractClassInformationService;
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

        var methods = types
            .OrderBy(x => x.Name)
            .SelectMany(x => CreateGenerationMethod(x, dependencies))
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
            .Prepend("/// This class was auto generated using TestDataFactoryGenerator, for following types:<br/>")
            .Prepend("/// <summary>")
            .Append("/// This class can be edited, but it is preferable to not touch this file, and extend the partial class in a separate file.")
            .Append("/// </summary>")
            .ToImmutableList();
    }

    private IImmutableList<string> GenerateStartOfClass(string tdfName)
    {
        var lines = new Lines(_config);

        List<(ushort Indents, string Line)> content =
        [
            (0, $"internal partial class {tdfName}"),
            (0, "{"),
            (1, $"private static readonly IImmutableList<int> {_config.LeadingUnderscore()}zeroBiasedCounts = [0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2];"),
            (0, string.Empty),
            (1, $"private int GetZeroBiasedCount() => {_config.LeadingUnderscore()}zeroBiasedCounts.OrderBy(_ => {_config.LeadingUnderscore()}random.Next()).First();"),
            (0, string.Empty),
            (1, $"private readonly Random {_config.LeadingUnderscore()}random;"),
            (0, string.Empty),
            (1, $"public {tdfName}()"),
            (1, $"{{"),
            (2, $"{_config.This()}random = new Random();"),
            (1, "}"),
            (0, string.Empty),
            (1, $"public {tdfName}(int seed)"),
            (1, $"{{"),
            (2, $"{_config.This()}random = new Random(seed);"),
            (1, "}"),
            (0, string.Empty),
            (1, $"public {tdfName}(Random random)"),
            (1, "{"),
            (2, $"{_config.This()}random = random;"),
            (1, "}"),
            (0, string.Empty),
        ];

        lines.AddRange(content.ToImmutableList());

        return lines.ToImmutableList();
    }

    public IImmutableList<string> CreateGenerationMethod(Type type, HashSet<string> dependencies)
    {
        if (type.IsEnum)
        {
            return ImmutableList<string>.Empty;
        }

        if (_randomizerCallerGenerator.CanGenerate(type))
        {
            return ImmutableList<string>.Empty;
        }

        if (_abstractClassInformationService.IsAbstractClassUsedAsOneOf(type))
        {
            Console.WriteLine("is abstract");
        }

        if (_eitherInformationService.IsEither(type))
        {
            return _eitherCodeGenerator.CreateGenerationCodeForEither(
                type,
                dependencies,
                _parameterInstantiationCodeGenerator);
        }

        return _typeWithConstructorsGenerator.CreateGenerationMethod(
            type,
            dependencies);
    }
}