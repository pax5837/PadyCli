namespace TestDataFactoryGenerator;

public class TdfGeneratorConfigurationOrPathToJson
{
    private readonly TdfGeneratorConfiguration? _configuration;
    private readonly string? _pathToJsonConfigFile;
    private readonly Discriminant _discriminant;

    public TdfGeneratorConfigurationOrPathToJson(TdfGeneratorConfiguration configuration)
    {
        _configuration = configuration;
        _discriminant = Discriminant.Config;
    }

    public TdfGeneratorConfigurationOrPathToJson(string pathToJsonConfigFile)
    {
        _pathToJsonConfigFile = pathToJsonConfigFile;
        _discriminant = Discriminant.Path;
    }
    
    public bool IsPath => _discriminant == Discriminant.Path;
    
    public bool IsConfiguration => _discriminant == Discriminant.Config;

    public TdfGeneratorConfiguration Switch(Func<string, TdfGeneratorConfiguration> whenPath)
    {
        return _discriminant switch
        {
            Discriminant.Path => whenPath(_pathToJsonConfigFile),
            Discriminant.Config => _configuration,
            _ => throw new InvalidOperationException(),
        };
    }
    
    private enum Discriminant
    {
        Config,
        Path,
    }
}