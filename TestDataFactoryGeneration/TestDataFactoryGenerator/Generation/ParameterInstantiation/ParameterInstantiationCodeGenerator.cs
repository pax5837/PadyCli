namespace TestDataFactoryGenerator.Generation.ParameterInstantiation;

internal class ParameterInstantiationCodeGenerator : IParameterInstantiationCodeGenerator
{
    private readonly IEitherCodeGenerator _eitherCodeGenerator;
    private readonly IEitherInformationService _eitherInformationService;
    private readonly IProtoCodeGenerator _protoCodeGenerator;
    private readonly IUserDefinedGenericsCodeGenerator _userDefinedGenericsCodeGenerator;
    private readonly IProtoInformationService _protoInformationService;
    private readonly ICollectionsCodeGenerator _collectionsCodeGenerator;
    private readonly IRandomizerCallerGenerator _randomizerCallerGenerator;

    public ParameterInstantiationCodeGenerator(
        IEitherCodeGenerator eitherCodeGenerator,
        IEitherInformationService eitherInformationService,
        IProtoCodeGenerator protoCodeGenerator,
        IUserDefinedGenericsCodeGenerator userDefinedGenericsCodeGenerator,
        IProtoInformationService protoInformationService,
        ICollectionsCodeGenerator collectionsCodeGenerator,
        IRandomizerCallerGenerator randomizerCallerGenerator)
    {
        _eitherCodeGenerator = eitherCodeGenerator;
        _eitherInformationService = eitherInformationService;
        _protoCodeGenerator = protoCodeGenerator;
        _userDefinedGenericsCodeGenerator = userDefinedGenericsCodeGenerator;
        _protoInformationService = protoInformationService;
        _collectionsCodeGenerator = collectionsCodeGenerator;
        _randomizerCallerGenerator = randomizerCallerGenerator;
    }

    public string GenerateParameterInstantiation(
        Type type,
        HashSet<string> dependencies,
        string? parameterName = null)
    {
        if (type.Namespace is not null && !_protoInformationService.IsProtoRepeatedField(type))
        {
            dependencies.Add(type.Namespace);
        }

        if (type.IsEnum)
        {
            return $"_random.NextEnum<{type.Name}>()";
        }

        if (_eitherInformationService.IsEither(type))
        {
            return _eitherCodeGenerator.GenerateEitherParameterInstantiation(type);
        }

        if (_randomizerCallerGenerator.CanGenerate(type))
        {
            return $"{_randomizerCallerGenerator.Generate(type, parameterName)}";
        }

        if (_protoInformationService.IsWellKnownProtobufType(type))
        {
            return _protoCodeGenerator.GenerateInstantiationForWellKnownProtobufType(type, dependencies);
        }

        if (type.Name.StartsWith("Nullable"))
        {
            var genericType = type.GenericTypeArguments.First();
            return GenerateParameterInstantiation(genericType, dependencies);
        }

        if (_protoInformationService.IsProtoRepeatedField(type))
        {
            return _protoCodeGenerator.GenerateInstantiationCodeForProtobufRepeatedType(type, dependencies, this);
        }

        if (_userDefinedGenericsCodeGenerator.IsAUserDefinedGenericType(type))
        {
            return _userDefinedGenericsCodeGenerator.GenerateInstantiationCode(type);
        }

        if (_collectionsCodeGenerator.IsACollection(type))
        {
            return _collectionsCodeGenerator.GenerateInstantiationCode(type, dependencies, this);
        }

        return $"{Definitions.GenerationMethodPrefix}{type.Name}()";
    }
}