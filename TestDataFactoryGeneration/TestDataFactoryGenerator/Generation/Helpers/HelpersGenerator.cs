using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation.Helpers;

internal class HelpersGenerator : IHelpersGenerator
{
    private readonly string indent;
    private readonly string thisField;
    private readonly string leadingUnderscore;

    public HelpersGenerator(TdfGeneratorConfiguration generatorConfiguration)
    {
        this.indent = generatorConfiguration.Indent;
        this.thisField = generatorConfiguration.This();
        this.leadingUnderscore = generatorConfiguration.LeadingUnderscore();
    }

    public IImmutableList<string> GenerateHelpersCode(bool includeHelperClasses)
    {
        if (!includeHelperClasses)
        {
            return [];
        }

        return GenerateNullObjectCode()
            .Prepend(string.Empty)
            .Concat(GenerateOptionCode())
            .Concat(GenerateOptionalRefCode())
            .Concat(GenerateOptionalValueCode())
            .Concat(GenerateRandomExtensions())
            .ToImmutableList();
    }

    private IImmutableList<string> GenerateNullObjectCode()
    {
        return
        [
            "internal class NullObject",
            "{",
            $"{indent}public static readonly NullObject Instance = new();",
            string.Empty,
            $"{indent}private NullObject()",
            $"{indent}{{",
            $"{indent}}}",
            "}",
            string.Empty,
        ];
    }

    private IImmutableList<string> GenerateOptionCode()
    {
        return
        [
            "internal enum Option",
            "{",
            $"{indent}AutoGenerated = 1,",
            $"{indent}Null = 2,",
            "}",
            string.Empty,
        ];
    }

    private IImmutableList<string> GenerateOptionalRefCode()
    {
        return
        [
            "internal struct OptionalRef<T> where T : class",
            "{",
            $"{indent}private readonly T? {leadingUnderscore}value;",
            string.Empty,
            $"{indent}private readonly InternalOption {leadingUnderscore}type = InternalOption.AutoGenerated;",
            string.Empty,
            $"{indent}public OptionalRef(T value)",
            $"{indent}{{",
            $"{indent}{indent}{thisField}value = value;",
            $"{indent}{indent}{thisField}type = InternalOption.SpecifiedValue;",
            $"{indent}}}",
            string.Empty,
            $"{indent}public OptionalRef(Option opt)",
            $"{indent}{{",
            $"{indent}{indent}{thisField}value = default;",
            $"{indent}{indent}{thisField}type = opt switch",
            $"{indent}{indent}{{",
            $"{indent}{indent}{indent}Option.Null => InternalOption.Null,",
            $"{indent}{indent}{indent}Option.AutoGenerated => InternalOption.AutoGenerated,",
            $"{indent}{indent}{indent}_ => throw new NotImplementedException($\"Can not handle {{opt}}\"),",
            $"{indent}{indent}}};",
            $"{indent}}}",
            string.Empty,
            $"{indent}public static implicit operator OptionalRef<T>(T value)",
            $"{indent}{{",
            $"{indent}{indent}return new OptionalRef<T>(value);",
            $"{indent}}}",
            string.Empty,
            $"{indent}public static implicit operator OptionalRef<T>(Option type)",
            $"{indent}{{",
            $"{indent}{indent}return new OptionalRef<T>(type);",
            $"{indent}}}",
            string.Empty,
            $"{indent}public T? Unwrap(Func<T> whenAutoGenerated)",
            $"{indent}{{",
            $"{indent}{indent}return {leadingUnderscore}type switch",
            $"{indent}{indent}{{",
            $"{indent}{indent}{indent}InternalOption.SpecifiedValue => {leadingUnderscore}value,",
            $"{indent}{indent}{indent}InternalOption.Null => null,",
            $"{indent}{indent}{indent}InternalOption.AutoGenerated => whenAutoGenerated(),",
            $"{indent}{indent}{indent}_ => throw new InvalidOperationException(),",
            $"{indent}{indent}}};",
            $"{indent}}}",
            string.Empty,
            $"{indent}private enum InternalOption",
            $"{indent}{{",
            $"{indent}{indent}AutoGenerated = 0,",
            $"{indent}{indent}SpecifiedValue = 1,",
            $"{indent}{indent}Null = 2,",
            $"{indent}}}",
            "}",
            string.Empty,
        ];
    }

    private IImmutableList<string> GenerateOptionalValueCode()
    {
        return
        [
            "internal struct OptionalValue<T> where T : struct",
            "{",
            $"{indent}private readonly T? {leadingUnderscore}value;",
            string.Empty,
            $"{indent}private readonly InternalOption {leadingUnderscore}type = InternalOption.AutoGenerated;",
            string.Empty,
            $"{indent}public OptionalValue(T value)",
            $"{indent}{{",
            $"{indent}{indent}{thisField}value = value;",
            $"{indent}{indent}{thisField}type = InternalOption.SpecifiedValue;",
            $"{indent}}}",
            string.Empty,
            $"{indent}public OptionalValue(Option opt)",
            $"{indent}{{",
            $"{indent}{indent}{thisField}value = default;",
            $"{indent}{indent}{thisField}type = opt switch",
            $"{indent}{indent}{{",
            $"{indent}{indent}{indent}Option.Null => InternalOption.Null,",
            $"{indent}{indent}{indent}Option.AutoGenerated => InternalOption.AutoGenerated,",
            $"{indent}{indent}{indent}_ => throw new NotImplementedException($\"Can not handle {{opt}}\"),",
            $"{indent}{indent}}};",
            $"{indent}}}",
            string.Empty,
            $"{indent}public static implicit operator OptionalValue<T>(T value)",
            $"{indent}{{",
            $"{indent}{indent}return new OptionalValue<T>(value);",
            $"{indent}}}",
            string.Empty,
            $"{indent}public static implicit operator OptionalValue<T>(Option type)",
            $"{indent}{{",
            $"{indent}{indent}return new OptionalValue<T>(type);",
            $"{indent}}}",
            string.Empty,
            $"{indent}public T? Unwrap(Func<T> whenAutoGenerated)",
            $"{indent}{{",
            $"{indent}{indent}return {leadingUnderscore}type switch",
            $"{indent}{indent}{{",
            $"{indent}{indent}{indent}InternalOption.SpecifiedValue => {leadingUnderscore}value,",
            $"{indent}{indent}{indent}InternalOption.Null => null,",
            $"{indent}{indent}{indent}InternalOption.AutoGenerated => whenAutoGenerated(),",
            $"{indent}{indent}{indent}_ => throw new InvalidOperationException(),",
            $"{indent}{indent}}};",
            $"{indent}}}",
            string.Empty,
            $"{indent}private enum InternalOption",
            $"{indent}{{",
            $"{indent}{indent}AutoGenerated = 0,",
            $"{indent}{indent}SpecifiedValue = 1,",
            $"{indent}{indent}Null = 2,",
            $"{indent}}}",
            "}",
            string.Empty,
        ];
    }

    private IImmutableList<string> GenerateRandomExtensions()
    {
        return
        [
            $"internal static class RandomExtensions",
            "{",
            $"{indent}public static string NextString(this Random r, string? prefix) => $\"{{prefix ?? \"RandomString\"}}_{{r.Next(1, 1_000_000)}}\";",
            string.Empty,
            $"{indent}public static byte[] NextBytes(this Random r, int length = 10)",
            $"{indent}{{",
            $"{indent}{indent}byte[] bytes = new byte[16];",
            $"{indent}{indent}r.NextBytes(bytes);",
            $"{indent}{indent}return bytes;",
            $"{indent}}}",
            string.Empty,
            $"{indent}public static Guid NextGuid(this Random r) => new Guid(r.NextBytes(16));",
            string.Empty,
            $"{indent}public static DateTimeOffset NextDateTimeOffset(this Random r) =>  new DateTimeOffset(r.NextInt64(), TimeSpan.FromHours(r.Next(-23, 23)));",
            string.Empty,
            $"{indent}public static TimeSpan NextTimeSpan(this Random r) => new TimeSpan(r.NextInt64());",
            string.Empty,
            $"{indent}public static DateTime NextDateTime(this Random r) => new DateTime(r.Next(2000, 2100), r.Next(1, 13), r.Next(1, 28), r.Next(0, 23), r.Next(0, 59), r.Next(0, 59));",
            string.Empty,
            $"{indent}public static bool NextBool(this Random r) => (r.Next(0, 1000) % 2) == 0;",
            string.Empty,
            $"{indent}public static long NextLong(this Random r) => r.NextInt64();",
            string.Empty,
            $"{indent}public static decimal NextDecimal(this Random r) => (decimal)r.NextDouble();",
            "}"
        ];
    }
}