namespace TestDataFactoryGenerator.Generation.Either;

internal class EitherInformationService : IEitherInformationService
{
    private readonly IConfiguration _configuration;

    public EitherInformationService(IConfiguration configuration)
    {
        _configuration = configuration;
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
               && type.Namespace.StartsWith(_configuration.EitherNamespace ?? string.Empty, StringComparison.OrdinalIgnoreCase)
               && type.Name.StartsWith("Either", StringComparison.OrdinalIgnoreCase);
    }
}