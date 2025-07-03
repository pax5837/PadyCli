using System.Collections.Immutable;

using Google.Protobuf;
using TdfGen;

namespace TestDataForTestDataFactoryGenerator;

/// <summary>
/// This class was auto generated using TestDataFactoryGenerator, for following types:<br/>
/// - <see cref="TdfGen.HelloRequest"/><br/>
/// - <see cref="TdfGen.HelloResponse"/><br/>
/// This class can be edited, but it is preferable to not touch this file, and extend the partial class in a separate file.
/// </summary>
internal partial class TdfProto
{
    private static readonly IImmutableList<int> zeroBiasedCounts = [0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2];

    private int GetZeroBiasedCount() => zeroBiasedCounts.OrderBy(_ => random.Next()).First();

    private readonly Random random;

    public TdfProto()
    {
        this.random = new Random();
    }

    public TdfProto(int seed)
    {
        this.random = new Random(seed);
    }

    public TdfProto(Random random)
    {
        this.random = random;
    }

    public ByteString GenerateByteString()
    {
        var constructorNumber = random.Next(0, 0);
        return constructorNumber switch {
            _ => throw new InvalidOperationException("Unexpected constructor number"),
        };
    }

    public Field GenerateField(
        string? key = null,
        string? label = null,
        string? stringValue = null,
        int? intValue = null,
        string? stringData = null,
        ByteString? bytes = null)
    {
        var generated = new Field
        {
            Key = key ?? random.NextString(""),
            Label = label ?? random.NextString(""),
            StringValue = stringValue ?? random.NextString(""),
            IntValue = intValue ?? random.Next(),
            StringData = stringData ?? random.NextString(""),
            Bytes = bytes ?? GenerateByteString(),
        };

        return generated;
    }

    public HelloRequest GenerateHelloRequest(
        string? message = null,
        IEnumerable<int>? someIntegers = null,
        long? optionalLong = null,
        IEnumerable<Field>? fields = null)
    {
        var generated = new HelloRequest
        {
            Message = message ?? random.NextString(""),
            OptionalLong = optionalLong ?? random.NextInt64(),
        };

        generated.SomeIntegers.AddRange(someIntegers ?? GetSome(() => random.Next()));
        generated.Fields.AddRange(fields ?? GetSome(() => GenerateField()));

        return generated;
    }

    public HelloResponse GenerateHelloResponse(
        string? responseMessage = null,
        Status? status = null)
    {
        var generated = new HelloResponse
        {
            ResponseMessage = responseMessage ?? random.NextString(""),
            Status = status ?? random.NextEnum<Status>(),
        };

        return generated;
    }

    public IEnumerable<T> GetSome<T>(Func<T> generator)
    {
        return Enumerable.Range(1, random.Next(0, GetZeroBiasedCount())).Select(_ => generator());
    }
}