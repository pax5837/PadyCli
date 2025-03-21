using System;
using System.Linq;
using System.Reflection;

namespace BuilderGenerator.Sources.Reflection;

internal static class ReflectionExtensions
{
    private const string NullableAttributeFullName = "System.Runtime.CompilerServices.NullableAttribute";
    private const string NullableContextAttributeFullName = "System.Runtime.CompilerServices.NullableContextAttribute";

    public static TypeKind GetKind(this ParameterInfo parameterInfo, NullabilityContextKind nullabilityContextKind)
    {
        var type = parameterInfo.ParameterType;
        if (type.IsValueType)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return new TypeKind(true, underlyingType != null);
        }

        var hasNullableAttribute = parameterInfo.CustomAttributes
            .Any(x => x.AttributeType.FullName == NullableAttributeFullName);

        var isNullable = nullabilityContextKind.Switch(
            () => hasNullableAttribute,
            () => !hasNullableAttribute,
            () => true);

        return new TypeKind(false, isNullable);
    }

    public static TypeKind GetKind(this PropertyInfo propertyInfo, NullabilityContextKind nullabilityContextKind)
    {
        var type = propertyInfo.PropertyType;
        if (type.IsValueType)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return new TypeKind(true, underlyingType != null);
        }

        var hasNullableAttribute = propertyInfo.CustomAttributes
            .Any(x => x.AttributeType.FullName == NullableAttributeFullName);

        var isNullable = nullabilityContextKind.Switch(
            () => hasNullableAttribute,
            () => !hasNullableAttribute,
            () => true);
        return new TypeKind(false, isNullable);
    }

    public static NullabilityContextKind GetNullabilityContextKind(this Type type)
    {
        var nullabilityContext = type.CustomAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == NullableContextAttributeFullName);

        if (nullabilityContext is null)
        {
            Console.WriteLine("no nullability context found -> none");
            return NullabilityContextKind.None;
        }

        var argumentValue = nullabilityContext.ConstructorArguments.FirstOrDefault().Value;

        if (argumentValue is not null && argumentValue is byte byteValue)
        {
            Console.WriteLine($"nullable context argument is {byteValue}");
            return byteValue == 1
                ? NullabilityContextKind.Standard
                : NullabilityContextKind.Inverted;
        }

        return NullabilityContextKind.Standard;
    }
}