namespace TestDataFactoryGenerator.Generation.ParameterInstantiation;

internal class ParameterInstantiationCodeGenerator : IParameterInstantiationCodeGenerator
{
    private readonly IEitherCodeGenerator _eitherCodeGenerator;
    private readonly IEitherInformationService _eitherInformationService;
    private readonly IProtoCodeGenerator _protoCodeGenerator;
    private readonly IUserDefinedGenericsCodeGenerator _userDefinedGenericsCodeGenerator;
    private readonly IProtoInformationService _protoInformationService;
    private readonly IDictionnariesCodeGenerator _dictionnariesCodeGenerator;
    private readonly ICollectionsCodeGenerator _collectionsCodeGenerator;
    private readonly ISimpleTypeGenerator _simpleTypeGenerator;
    private readonly INamespaceAliasManager _namespaceAliasManager;
    private readonly TdfGeneratorConfiguration _config;

    public ParameterInstantiationCodeGenerator(
        IEitherCodeGenerator eitherCodeGenerator,
        IEitherInformationService eitherInformationService,
        IProtoCodeGenerator protoCodeGenerator,
        IUserDefinedGenericsCodeGenerator userDefinedGenericsCodeGenerator,
        IProtoInformationService protoInformationService,
        IDictionnariesCodeGenerator  dictionnariesCodeGenerator,
        ICollectionsCodeGenerator collectionsCodeGenerator,
        ISimpleTypeGenerator simpleTypeGenerator,
        INamespaceAliasManager namespaceAliasManager,
        TdfGeneratorConfiguration config)
    {
        _eitherCodeGenerator = eitherCodeGenerator;
        _eitherInformationService = eitherInformationService;
        _protoCodeGenerator = protoCodeGenerator;
        _userDefinedGenericsCodeGenerator = userDefinedGenericsCodeGenerator;
        _protoInformationService = protoInformationService;
        _dictionnariesCodeGenerator = dictionnariesCodeGenerator;
        _collectionsCodeGenerator = collectionsCodeGenerator;
        _simpleTypeGenerator = simpleTypeGenerator;
        _namespaceAliasManager = namespaceAliasManager;
        _config = config;
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
            return $"{_config.LeadingUnderscore()}random.NextEnum<{_namespaceAliasManager.GetNamespaceAliasWithDot(type)}{type.Name}>()";
        }

        if (_eitherInformationService.IsEither(type))
        {
            return _eitherCodeGenerator.GenerateEitherParameterInstantiation(type);
        }

        if (_simpleTypeGenerator.CanGenerate(type))
        {
            return $"{_simpleTypeGenerator.Generate(type, parameterName)}";
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

        if (_dictionnariesCodeGenerator.IsADictionary(type))
        {
            return _dictionnariesCodeGenerator.GenerateInstantiationCode(type, dependencies, this);
        }

        if (_collectionsCodeGenerator.IsACollection(type))
        {
            return _collectionsCodeGenerator.GenerateInstantiationCode(type, dependencies, this);
        }

        return $"{Definitions.GenerationMethodPrefix}{type.Name}()";
    }
}