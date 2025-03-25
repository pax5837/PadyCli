using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Either;

internal class EitherCodeGenerator : IEitherCodeGenerator
{
    private readonly IEitherInformationService _eitherInformationService;
    private readonly ITypeNameGenerator _typeNameGenerator;
    private readonly IConfiguration _configuration;

    public EitherCodeGenerator(
        IEitherInformationService eitherInformationService,
        ITypeNameGenerator typeNameGenerator,
        IConfiguration configuration)
    {
        _eitherInformationService = eitherInformationService;
        _typeNameGenerator = typeNameGenerator;
        _configuration = configuration;
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
        if (!string.IsNullOrWhiteSpace(_configuration.EitherNamespace))
        {
            dependencies.Add(_configuration.EitherNamespace);
        }

        var lines = new List<string>();

        var genericArguments = type.GenericTypeArguments.ToImmutableList();
        var genericArgumentsCount = genericArguments.Count;

        var eitherTypeAsString = _eitherInformationService.GetEitherTypeAsString(type, _typeNameGenerator.GetTypeNameForParameter);

        lines.Add(
            $"\tpublic {eitherTypeAsString} {Definitions.GenerationMethodPrefix}{GetEitherGeneratorName(type)}()");
        lines.Add("\t{");
        lines.Add($"\t\tvar genericTypeNumber = _random.Next(0, {genericArgumentsCount});");
        lines.Add($"\t\treturn genericTypeNumber switch {{");
        for (int i = 0; i < genericArgumentsCount; i++)
        {
            var parameterInstantiation = parameterInstantiationCodeGenerator
                .GenerateParameterInstantiation(genericArguments[i], dependencies);
            lines.Add($"\t\t\t{i} => {eitherTypeAsString}.From({parameterInstantiation}),");
        }

        lines.Add("\t\t\t_ => throw new InvalidOperationException(\"Unexpected constructor number\"),");
        lines.Add("\t\t};");
        lines.Add("\t}");
        lines.Add(string.Empty);

        return lines.ToImmutableList();
    }
}