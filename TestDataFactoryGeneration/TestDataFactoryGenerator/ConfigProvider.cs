using System.Text.Json;

namespace TestDataFactoryGenerator;

internal static class ConfigProvider
{
    public static TdfGeneratorConfiguration GetConfiguration(TdfGeneratorConfigurationOrPathToJson? externalConfiguration)
    {
        if (externalConfiguration is not null)
        {
            return externalConfiguration.Switch(whenPath: GetConfigFromJsonFile);
        }

        var pathToConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PadyCli\\TdfGeneratorConfig.json");

        var config  = GetConfigFromJsonFile(pathToConfig);
        if (config is not null)
        {
            return config;
        }

        return GetDefaultTdfGeneratorConfig();
    }

    private static TdfGeneratorConfiguration? GetConfigFromJsonFile(string pathToConfig)
    {
        if (File.Exists(pathToConfig))
        {
            var fileContent = File.ReadAllText(pathToConfig);
            return JsonSerializer.Deserialize<TdfGeneratorConfiguration>(fileContent)!;
        }

        return null;
    }

    private static TdfGeneratorConfiguration GetDefaultTdfGeneratorConfig()
    {
        var simpleTypeConfiguration = new SimpleTypeConfiguration(
            "#########",
            [
                new(typeof(string), "GenerateRandomString(\"#########\")", "System.String", ["private string GenerateRandomString(string? parameterName) => $\"{parameterName ?? \"SomeString\"}_{_random.Next(1, int.MaxValue)}\";"]),
                new(typeof(int), "GenerateRandomInt()", null, ["private int GenerateRandomInt() => _random.Next();"]),
                new(typeof(Guid), "GenerateRandomGuid()", null, ["private Guid GenerateRandomGuid() => Guid.NewGuid();"]),
                new(typeof(DateTimeOffset), "GenerateRandomDateTimeOffset()", null, ["private DateTimeOffset GenerateRandomDateTimeOffset() => new DateTimeOffset(_random.NextInt64(), TimeSpan.FromHours(_random.Next(-23, 23)));"]),
                new(typeof(TimeSpan), "GenerateRandomTimeSpan()", null, ["private TimeSpan GenerateRandomTimeSpan() => new TimeSpan(_random.NextInt64());"]),
                new(typeof(bool), "GenerateRandomBool()", null, ["private bool GenerateRandomBool() => _random.Next() % 2 == 0;"]),
                new(typeof(long), "GenerateRandomLong()", null, ["private long GenerateRandomLong() => _random.NextInt64();"]),
                new(typeof(decimal), "GenerateRandomDecimal()", null, ["private decimal GenerateRandomDecimal() => (decimal)_random.NextDouble();"]),
            ]);

        var tdfGeneratorConfiguration1 = new TdfGeneratorConfiguration(
            NamespacesToAdd: [],
            Indent: "    ",
            EitherNamespace: null,
            CustomInstantiationForWellKnownProtobufTypes: [],
            SimpleTypeConfiguration: simpleTypeConfiguration);
        return tdfGeneratorConfiguration1;
    }
}