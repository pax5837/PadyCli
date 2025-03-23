using TestDataFactoryGenerator.Generation.Protobuf;

namespace TestDataFactoryGenerator.Generation.ParameterInstantiation;

internal class ParameterInstantiationCodeGenerator : IParameterInstantiationCodeGenerator
{
    private readonly IEitherCodeGenerator _eitherCodeGenerator;
    private readonly IEitherInformationService _eitherInformationService;
    private readonly IProtoCodeGenerator _protoCodeGenerator;
    private readonly IUserDefinedGenericsCodeGenerator _userDefinedGenericsCodeGenerator;
    private readonly IProtoInformationService _protoInformationService;
    private readonly ICollectionsCodeGenerator _collectionsCodeGenerator;

    public ParameterInstantiationCodeGenerator(
        IEitherCodeGenerator eitherCodeGenerator,
        IEitherInformationService eitherInformationService,
        IProtoCodeGenerator protoCodeGenerator,
        IUserDefinedGenericsCodeGenerator userDefinedGenericsCodeGenerator,
        IProtoInformationService protoInformationService,
        ICollectionsCodeGenerator collectionsCodeGenerator)
    {
        _eitherCodeGenerator = eitherCodeGenerator;
        _eitherInformationService = eitherInformationService;
        _protoCodeGenerator = protoCodeGenerator;
        _userDefinedGenericsCodeGenerator = userDefinedGenericsCodeGenerator;
        _protoInformationService = protoInformationService;
        _collectionsCodeGenerator = collectionsCodeGenerator;
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

        if (RandomizerCallerGenerator.CanGenerate(type))
        {
            return $"{RandomizerCallerGenerator.Generate(type, parameterName)}";
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
            return ProtoCodeGenerator.GenerateInstantiationCodeForProtobufRepeatedType(type, dependencies, this);
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