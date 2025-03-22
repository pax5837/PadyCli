using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.TypeNames;

internal class TypeNameGenerator : ITypeNameGenerator
{
    private readonly IEitherInformationService _eitherInformationService;
    private readonly IProtoInformationService _protoInformationService;

    private static readonly IImmutableDictionary<Type, string> TypeMap = new (Type, string)[]
    {
        (typeof(string), "string"),
        (typeof(bool), "bool"),
        (typeof(int), "int"),
        (typeof(uint), "uint"),
        (typeof(long), "long"),
        (typeof(ulong), "ulong"),
        (typeof(short), "short"),
        (typeof(ushort), "ushort"),
        (typeof(float), "float"),
        (typeof(double), "double"),
        (typeof(decimal), "decimal"),
        (typeof(byte), "byte"),
        (typeof(byte[]), "byte[]"),
    }.ToImmutableDictionary(x => x.Item1, x => x.Item2);

    public TypeNameGenerator(
        IEitherInformationService eitherInformationService,
        IProtoInformationService protoInformationService)
    {
        _eitherInformationService = eitherInformationService;
        _protoInformationService = protoInformationService;
    }

    public string GetTypeNameForParameter(Type parameterType)
    {
        if (TypeMap.Keys.Contains(parameterType))
        {
            return TypeMap[parameterType];
        }

        if (_eitherInformationService.IsEither(parameterType))
        {
            return _eitherInformationService.GetEitherTypeAsString(parameterType, GetTypeNameForParameter);
        }

        if (parameterType.Name.StartsWith("Nullable"))
        {
            var genericType = parameterType.GenericTypeArguments[0];
            return GetTypeNameForParameter(genericType);
        }

        if (_protoInformationService.IsProtoRepeatedField(parameterType))
        {
            var genericTypeName = GetTypeNameForParameter(parameterType.GenericTypeArguments.Single());
            return _protoInformationService.GetParameterTypeNameForRepeatedField(genericTypeName);
        }

        if (parameterType.IsGenericType)
        {
            var className = parameterType.Name
                .Split('`', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).First();
            return
                $"{className}<{string.Join(", ", parameterType.GenericTypeArguments.Select(GetTypeNameForParameter))}>";
        }

        return parameterType.Name;
    }
}