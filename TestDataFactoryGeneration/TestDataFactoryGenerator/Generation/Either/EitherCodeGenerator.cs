using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Either;

internal class EitherCodeGenerator : IEitherCodeGenerator
{
    private readonly IEitherInformationService _eitherInformationService;
    private readonly ITypeNameGenerator _typeNameGenerator;
    private readonly TdfGeneratorConfiguration _config;

    public EitherCodeGenerator(
        IEitherInformationService eitherInformationService,
        ITypeNameGenerator typeNameGenerator,
        TdfGeneratorConfiguration config)
    {
        _eitherInformationService = eitherInformationService;
        _typeNameGenerator = typeNameGenerator;
        _config = config;
    }

    public string GetEitherGeneratorName(Type type)
    {
        var genericTypeArguments = string.Join(
            string.Empty,
            type.GenericTypeArguments.Select(gta => gta.Name));

        return $"Either{genericTypeArguments}";
    }

    public string GenerateEitherParameterInstantiation(Type type)
    {
        return $"{Definitions.GenerationMethodPrefix}{GetEitherGeneratorName(type)}()";
    }

    public IImmutableList<string> CreateGenerationCodeForEither(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator)
    {
        if (!string.IsNullOrWhiteSpace(_config.EitherNamespace))
        {
            dependencies.Add(_config.EitherNamespace);
        }

        var lines = new List<string>();

        var genericArguments = type.GenericTypeArguments.ToImmutableList();
        var genericArgumentsCount = genericArguments.Count;

        var eitherTypeAsString = _eitherInformationService.GetEitherTypeAsString(type, _typeNameGenerator.GetTypeNameForParameter);

        lines.Add(
            $"{_config.Indent}public {eitherTypeAsString} {Definitions.GenerationMethodPrefix}{GetEitherGeneratorName(type)}()");
        lines.Add($"{_config.Indent}{{");
        lines.Add($"{_config.Indent}{_config.Indent}var genericTypeNumber = _random.Next(0, {genericArgumentsCount});");
        lines.Add($"{_config.Indent}{_config.Indent}return genericTypeNumber switch {{");
        for (int i = 0; i < genericArgumentsCount; i++)
        {
            var parameterInstantiation = parameterInstantiationCodeGenerator
                .GenerateParameterInstantiation(genericArguments[i], dependencies);
            lines.Add($"{_config.Indent}{_config.Indent}{_config.Indent}{i} => {eitherTypeAsString}.From({parameterInstantiation}),");
        }

        lines.Add($"{_config.Indent}{_config.Indent}{_config.Indent}_ => throw new InvalidOperationException(\"Unexpected constructor number\"),");
        lines.Add($"{_config.Indent}{_config.Indent}}};");
        lines.Add($"{_config.Indent}}}");
        lines.Add(string.Empty);

        return lines.ToImmutableList();
    }
}