using System.Collections.Immutable;
using System.Text.Json;

namespace TestDataFactoryGenerator.Generation;

internal static class ConfigProvider
{
    public static TdfGeneratorConfiguration GetConfiguration(TdfConfigDefinition? externalConfiguration)
    {
        if (externalConfiguration is not null)
        {
            return externalConfiguration.Get(whenPath: GetConfigFromJsonFile);
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
        var useLeadingUnderscoreForPrivateFields = false;

        var randomField =  useLeadingUnderscoreForPrivateFields ? "_random" : "random";

        ImmutableList<InstantiationConfigurationForType> instantiationConfigurationForTypes =
        [
            new(typeof(string), $"{randomField}.NextString(\"#########\")", "System.String", []),
            new(typeof(int), $"{randomField}.Next()", null, []),
            new(typeof(Guid), $"{randomField}.NextGuid()", null, []),
            new(typeof(DateTimeOffset), $"{randomField}.NextDateTimeOffset()", null, []),
            new(typeof(DateTime), $"{randomField}.NextDateTime()", null, []),
            new(typeof(TimeSpan), $"{randomField}.NextTimeSpan()", null, []),
            new(typeof(bool), $"{randomField}.NextBool()", null, []),
            new(typeof(long), $"{randomField}.NextInt64()", null, []),
            new(typeof(decimal), $"{randomField}.NextDecimal()", null, []),
        ];

        var simpleTypeConfiguration = new SimpleTypeConfiguration(
            ParameterNamePlaceholder: "#########",
            InstantiationConfigurations: instantiationConfigurationForTypes
                .Select(selector: x => x with { MethodCodeToAdd = x.MethodCodeToAdd.Select(selector: c => useLeadingUnderscoreForPrivateFields ? c : c.Replace(oldValue: "_random", newValue: "random"))
                    .ToImmutableList() } )
                .ToImmutableList());

        var tdfGeneratorConfiguration1 = new TdfGeneratorConfiguration(
            NamespacesToAdd: [],
            Indent: "    ",
            EitherNamespace: null,
            CustomInstantiationForWellKnownProtobufTypes: [],
            SimpleTypeConfiguration: simpleTypeConfiguration,
            UseLeadingUnderscoreForPrivateFields: useLeadingUnderscoreForPrivateFields);
        return tdfGeneratorConfiguration1;
    }
}