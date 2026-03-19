using TestDataFactoryGenerator;

namespace PadyCliInteractiveConsoleApp.Features.TestDataFactoryGeneration;

internal class TestDataFactoryConfiguration
{
    public static TdfConfigDefinition GetTdfConfig()
    {
        var useLeadingUnderscoreForPrivateFields = true;

        var randomField = useLeadingUnderscoreForPrivateFields ? "_random" : "random";


        var simpleTypeConfiguration = new SimpleTypeConfiguration(
            ParameterNamePlaceholder: "#########",
            InstantiationConfigurations:
            [
                new(typeof(string), $"{randomField}.NextString()", "System.String", []),
                new(typeof(int), $"{randomField}.Next()", null, []),
                new(typeof(Guid), $"{randomField}.NextGuid()", null, []),
                new(typeof(DateTimeOffset), $"{randomField}.NextDateTimeOffset()", null, []),
                new(typeof(DateTime), $"{randomField}.NextDateTime()", null, []),
                new(typeof(TimeSpan), $"{randomField}.NextTimeSpan()", null, []),
                new(typeof(bool), $"{randomField}.NextBool()", null, []),
                new(typeof(long), $"{randomField}.NextInt64()", null, []),
                new(typeof(decimal), $"{randomField}.NextDecimal()", null, []),
            ]);

        var tdfGeneratorConfiguration = new TdfGeneratorConfiguration(
            NamespacesToAdd: [],
            Indent: "\t",
            EitherNamespace: null,
            CustomInstantiationForWellKnownProtobufTypes: [],
            SimpleTypeConfiguration: simpleTypeConfiguration,
            UseLeadingUnderscoreForPrivateFields: useLeadingUnderscoreForPrivateFields);
        return TdfConfigDefinition.FromConfig(tdfGeneratorConfiguration);
    }
}