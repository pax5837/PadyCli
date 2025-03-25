namespace TestDataFactoryGenerator.Generation;

internal class RandomizerCallerGenerator : IRandomizerCallerGenerator
{
    private readonly TdfGeneratorConfiguration _config;

    public RandomizerCallerGenerator(TdfGeneratorConfiguration config)
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
        return _config.SimpleTypeConfiguration.InstantiationConfigurationForTypeByType[type].InstantiationCode.Replace(_config.SimpleTypeConfiguration.ParameterNamePlaceholder, parameterName ?? string.Empty);
    }
}