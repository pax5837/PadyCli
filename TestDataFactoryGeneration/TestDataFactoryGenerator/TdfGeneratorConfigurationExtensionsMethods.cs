namespace TestDataFactoryGenerator;

internal static class TdfGeneratorConfigurationExtensionsMethods
{
    public static string LeadingUnderscore(this TdfGeneratorConfiguration config)
        => config.UseLeadingUnderscoreForPrivateFields ? "_" : string.Empty;

    public static string This(this TdfGeneratorConfiguration config)
        => config.UseLeadingUnderscoreForPrivateFields ? "_" : "this.";
}