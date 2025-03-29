namespace TestDataFactoryGenerator.Generation.SimpleTypeGenerator;

internal class SimpleTypeGenerator : ISimpleTypeGenerator
{
    private readonly TdfGeneratorConfiguration _config;

    public SimpleTypeGenerator(TdfGeneratorConfiguration config)
    {
        _config = config;
    }

    public bool CanGenerate(Type type)
    {
        return _config.SimpleTypeConfiguration.InstantiationConfigurationForTypeByType.ContainsKey(type);
    }

    public string Generate(
        Type type,
        string? parameterName)
    {
        return _config.SimpleTypeConfiguration
            .InstantiationConfigurationForTypeByType[type]
            .InstantiationCode
            .Replace(_config.SimpleTypeConfiguration.ParameterNamePlaceholder, parameterName ?? string.Empty);
    }
}