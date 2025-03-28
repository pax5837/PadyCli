namespace TestDataFactoryGenerator;

internal static class TdfGeneratorConfigurationExtensionsMethods
{
    public static string LeadingUnderscore(this TdfGeneratorConfiguration config)
        => config.UseLeadingUnderscoreForPrivateFields ? "_" : string.Empty;

    public static string This(this TdfGeneratorConfiguration config)
        => config.UseLeadingUnderscoreForPrivateFields ? "_" : "this.";

    public static string Indents(this TdfGeneratorConfiguration config, ushort count)
        => string.Join(string.Empty, Enumerable.Range(0, count).Select(_ => config.Indent));
}