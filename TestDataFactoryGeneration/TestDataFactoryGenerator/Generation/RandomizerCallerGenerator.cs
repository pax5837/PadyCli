using System.Collections.Immutable;

namespace TestDataFactoryGenerator.Generation;

internal class RandomizerCallerGenerator : IRandomizerCallerGenerator
{
    private const string ParameterNamePlaceholder = "###########################";

    private readonly IImmutableDictionary<Type, string> GenerationCode = new[]
    {
        (typeof(string), $"_random.NextString({ParameterNamePlaceholder})"),
        (typeof(int), "_random.Next()"),
        (typeof(Guid), "_random.NextGuid()"),
        (typeof(DateTimeOffset), "_random.NextDateTimeOffset()"),
        (typeof(TimeSpan), "_random.NextTimeSpan()"),
        (typeof(byte), "_random.NextByte()"),
        (typeof(byte[]), "_random.NextByteArray()"),
        (typeof(bool), "_random.NextBool()"),
        (typeof(long), "_random.NextInt64()"),
        (typeof(decimal), "_random.NextDecimal()"),
    }.ToImmutableDictionary(x => x.Item1, x => x.Item2);

    public bool CanGenerate(Type type)
    {
        return GenerationCode.ContainsKey(type);
    }

    public string Generate(
        Type type,
        string? parameterName)
    {
        return GenerationCode[type].Replace(ParameterNamePlaceholder, parameterName ?? string.Empty);
    }
}