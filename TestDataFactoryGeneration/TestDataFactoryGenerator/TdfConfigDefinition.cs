namespace TestDataFactoryGenerator;

public class TdfConfigDefinition
{
    private readonly TdfGeneratorConfiguration? _configuration;
    private readonly string? _pathToJsonConfigFile;
    private readonly Discriminant _discriminant;

    private TdfConfigDefinition(TdfGeneratorConfiguration configuration)
    {
        _configuration = configuration;
        _discriminant = Discriminant.Config;
    }

    private TdfConfigDefinition(string pathToJsonConfigFile)
    {
        _pathToJsonConfigFile = pathToJsonConfigFile;
        _discriminant = Discriminant.Path;
    }

    public bool IsPath => _discriminant == Discriminant.Path;

    public bool IsConfiguration => _discriminant == Discriminant.Config;

    public TdfGeneratorConfiguration Get(Func<string, TdfGeneratorConfiguration> whenPath)
    {
        return _discriminant switch
        {
            Discriminant.Path => whenPath(_pathToJsonConfigFile),
            Discriminant.Config => _configuration,
            _ => throw new InvalidOperationException(),
        };
    }

    public static TdfConfigDefinition FromJsonFile(string jsonFilePath) => new(jsonFilePath);
    public static TdfConfigDefinition FromConfig(TdfGeneratorConfiguration config) => new(config);

    private enum Discriminant
    {
        Config,
        Path,
    }
}