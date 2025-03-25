namespace TestDataFactoryGenerator.Generation.Either;

internal class EitherInformationService : IEitherInformationService
{
    private readonly TdfGeneratorConfiguration _tdfGeneratorConfiguration;

    public EitherInformationService(TdfGeneratorConfiguration tdfGeneratorConfiguration)
    {
        _tdfGeneratorConfiguration = tdfGeneratorConfiguration;
    }

    public string GetEitherTypeAsString(Type type, Func<Type, string> genericTypeNameGenerator)
    {
        var genericTypeArguments = string.Join(
            ", ",
            type.GenericTypeArguments.Select(genericTypeNameGenerator));

        return $"Either<{genericTypeArguments}>";
    }

    public bool IsEither(Type type)
    {

        return type.Namespace is not null
               && type.Namespace.StartsWith(_tdfGeneratorConfiguration.EitherNamespace ?? string.Empty, StringComparison.OrdinalIgnoreCase)
               && type.Name.StartsWith("Either", StringComparison.OrdinalIgnoreCase);
    }
}