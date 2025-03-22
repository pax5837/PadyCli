using System.Reflection;

namespace TestDataFactoryGenerator.Generation;

internal static class ReflectionExtensions
{
    public static TypeKind GetKind(this ParameterInfo parameterInfo, NullabilityKind nullabilityKind)
    {
        var type = parameterInfo.ParameterType;
        if (type.IsValueType)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return new TypeKind(true, underlyingType != null);
        }

        var hasNullableAttribute = parameterInfo.CustomAttributes
            .Any(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

        var isNullable = nullabilityKind switch
        {
            NullabilityKind.Inverted => !hasNullableAttribute,
            NullabilityKind.Standard => hasNullableAttribute,
            _ => throw new InvalidOperationException($"NullabilityKind {nullabilityKind} is not handled"),
        };

        return new TypeKind(false, isNullable);
    }

    public static TypeKind GetKind(this PropertyInfo propertyInfo, NullabilityKind nullabilityKind)
    {
        var type = propertyInfo.PropertyType;
        if (type.IsValueType)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return new TypeKind(true, underlyingType != null);
        }

        var hasNullableAttribute = propertyInfo.CustomAttributes
            .Any(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

        var isNullable = nullabilityKind switch
        {
            NullabilityKind.Inverted => !hasNullableAttribute,
            NullabilityKind.Standard => hasNullableAttribute,
            _ => throw new InvalidOperationException($"NullabilityKind {nullabilityKind} is not handled"),
        };

        return new TypeKind(false, isNullable);
    }

    public static NullabilityKind GetNullabilityKind(this Type type)
    {
        var nullableArgument = type.CustomAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute")
            ?
            .ConstructorArguments
            .First();

        if (nullableArgument is not null && nullableArgument.Value.Value is byte nullableArgumentValue)
        {
            if (nullableArgumentValue == 2)
            {
                return NullabilityKind.Inverted;
            }

            return NullabilityKind.Standard;
        }

        return NullabilityKind.Standard;
    }
}

internal record TypeKind(bool IsValueType, bool IsNullable);

internal enum NullabilityKind
{
    Standard,
    Inverted,
}