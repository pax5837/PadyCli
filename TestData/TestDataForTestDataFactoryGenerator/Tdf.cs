using System.Collections.Frozen;
using System.Collections.Immutable;

using TestDataForTestDataFactoryGenerator.Served;

namespace TestDataForTestDataFactoryGenerator;

/// <summary>
/// This class was auto generated using TestDataFactoryGenerator, for following types:<br/>
/// - <see cref="TestDataForTestDataFactoryGenerator.Served.Delivery"/><br/>
/// - <see cref="TestDataForTestDataFactoryGenerator.Served.Order"/><br/>
/// This class can be edited, but it is preferable to not touch this file, and extend the partial class in a separate file.
/// </summary>
internal partial class MyTdf
{
    private static readonly IImmutableList<int> zeroBiasedCounts = [0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2];

    private int GetZeroBiasedCount() => zeroBiasedCounts.OrderBy(_ => random.Next()).First();

    private readonly Random random;

    public MyTdf()
    {
        this.random = new Random();
    }

    public MyTdf(int seed)
    {
        this.random = new Random(seed);
    }

    public MyTdf(Random random)
    {
        this.random = random;
    }

    public AllCollections GenerateAllCollections(
        ImmutableArray<PositionNote>? positionNoteImmutableArray = null,
        PositionNote[]? positionNoteArray = null,
        List<PositionNote>? positionNoteList = null,
        ImmutableList<PositionNote>? positionNoteImmutableList = null,
        IImmutableList<PositionNote>? positionNoteIImmutableList = null,
        IReadOnlyList<PositionNote>? positionNoteIReadOnlyList = null,
        IImmutableSet<PositionNote>? positionNoteIImmutableSet = null,
        ImmutableHashSet<PositionNote>? positionNoteImmutableHAshSet = null,
        ImmutableSortedSet<PositionNote>? positionNoteImmutableSortedSet = null,
        IReadOnlySet<PositionNote>? positionNoteIReadOnlySet = null,
        FrozenSet<PositionNote>? positionNoteFrozenSet = null,
        IImmutableDictionary<Guid, PositionNote>? positionNoteIImmutableDictionary = null,
        ImmutableDictionary<Guid, PositionNote>? positionNoteImmutableDictionary = null,
        ImmutableSortedDictionary<Guid, PositionNote>? positionNoteImmutableSortedDictionary = null,
        FrozenDictionary<Guid, PositionNote>? positionNoteFrozenDictionary = null,
        Dictionary<Guid, PositionNote>? positionNoteDictionary = null)
    {
        return new AllCollections(
            PositionNoteImmutableArray: positionNoteImmutableArray ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteArray: positionNoteArray ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteList: positionNoteList ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteImmutableList: positionNoteImmutableList ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIImmutableList: positionNoteIImmutableList ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIReadOnlyList: positionNoteIReadOnlyList ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIImmutableSet: positionNoteIImmutableSet ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteImmutableHAshSet: positionNoteImmutableHAshSet ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteImmutableSortedSet: positionNoteImmutableSortedSet ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIReadOnlySet: positionNoteIReadOnlySet ?? GetSome(() => GeneratePositionNote()).ToHashSet(),
            PositionNoteFrozenSet: positionNoteFrozenSet ?? GetSome(() => GeneratePositionNote()).ToFrozenSet(),
            PositionNoteIImmutableDictionary: positionNoteIImmutableDictionary ?? GetSome(() => (Key: random.NextGuid(), Value: GeneratePositionNote())).ToImmutableDictionary(x => x.Key, x => x.Value),
            PositionNoteImmutableDictionary: positionNoteImmutableDictionary ?? GetSome(() => (Key: random.NextGuid(), Value: GeneratePositionNote())).ToImmutableDictionary(x => x.Key, x => x.Value),
            PositionNoteImmutableSortedDictionary: positionNoteImmutableSortedDictionary ?? GetSome(() => (Key: random.NextGuid(), Value: GeneratePositionNote())).ToImmutableSortedDictionary(x => x.Key, x => x.Value),
            PositionNoteFrozenDictionary: positionNoteFrozenDictionary ?? GetSome(() => (Key: random.NextGuid(), Value: GeneratePositionNote())).ToFrozenDictionary(x => x.Key, x => x.Value),
            PositionNoteDictionary: positionNoteDictionary ?? GetSome(() => (Key: random.NextGuid(), Value: GeneratePositionNote())).ToDictionary(x => x.Key, x => x.Value));
    }

    public ContactInfo GenerateContactInfo()
    {
        return random.Next(0, 2) switch {
            0 => GeneratePhoneContactInfo(),
            1 => GenerateEmailContactInfo(),
            _ => throw new InvalidOperationException("Unexpected constructor number"),
        };
    }

    public Delivery GenerateDelivery(
        IImmutableList<ItemPosition>? items = null,
        DateTimeOffset? deliveryDate = null)
    {
        return new Delivery(
            Items: items ?? [..GetSome(() => GenerateItemPosition())],
            DeliveryDate: deliveryDate ?? random.NextDateTimeOffset());
    }

    public EmailContactInfo GenerateEmailContactInfo(
        Person? person = null,
        string? email = null)
    {
        return new EmailContactInfo(
            Person: person ?? GeneratePerson(),
            Email: email ?? random.NextString(""));
    }

    public ItemPosition GenerateItemPosition(
        Guid? itemPositionId = null,
        Guid? productId = null,
        string? productShortName = null,
        int? itemCount = null,
        decimal? positionPrice = null)
    {
        return new ItemPosition(
            ItemPositionId: itemPositionId ?? random.NextGuid(),
            ProductId: productId ?? random.NextGuid(),
            ProductShortName: productShortName ?? random.NextString(""),
            ItemCount: itemCount ?? random.Next(),
            PositionPrice: positionPrice ?? random.NextDecimal());
    }

    public Order GenerateOrder(
        Guid? orderId = null,
        DateTimeOffset? orderDate = null,
        IImmutableSet<Position>? position = null,
        ContactInfo? contactInfo = null,
        Delivery? delivery = null,
        IImmutableDictionary<Guid, PriceInformation>? priceByProductId = null,
        AllCollections? allCollections = null)
    {
        return new Order(
            OrderId: orderId ?? random.NextGuid(),
            OrderDate: orderDate ?? random.NextDateTimeOffset(),
            Position: position ?? [..GetSome(() => GeneratePosition())],
            ContactInfo: contactInfo ?? GenerateContactInfo(),
            Delivery: delivery ?? GenerateDelivery(),
            PriceByProductId: priceByProductId ?? GetSome(() => (Key: random.NextGuid(), Value: GeneratePriceInformation())).ToImmutableDictionary(x => x.Key, x => x.Value),
            AllCollections: allCollections ?? GenerateAllCollections());
    }

    public Person GeneratePerson(
        Guid? id = null,
        string? firstName = null,
        string? lastName = null)
    {
        return new Person(
            Id: id ?? random.NextGuid(),
            FirstName: firstName ?? random.NextString(""),
            LastName: lastName ?? random.NextString(""));
    }

    public PhoneContactInfo GeneratePhoneContactInfo(
        Person? person = null,
        string? phoneNumber = null)
    {
        return new PhoneContactInfo(
            Person: person ?? GeneratePerson(),
            PhoneNumber: phoneNumber ?? random.NextString(""));
    }

    public Position GeneratePosition(
        Guid? positionId = null,
        Guid? productId = null,
        int? count = null,
        OptionalRef<PositionNote> positionNote = default,
        OptionalValue<int> batchSize = default)
    {
        return new Position(
            PositionId: positionId ?? random.NextGuid(),
            ProductId: productId ?? random.NextGuid(),
            Count: count ?? random.Next(),
            PositionNote: positionNote.Unwrap(whenAutoGenerated: () => GeneratePositionNote()),
            BatchSize: batchSize.Unwrap(whenAutoGenerated: () => random.Next()));
    }

    public PositionNote GeneratePositionNote(
        string? note = null)
    {
        return new PositionNote(
            Note: note ?? random.NextString(""));
    }

    public PriceInformation GeneratePriceInformation(
        decimal? basePrice = null,
        IImmutableDictionary<int, decimal>? discountPercentByCount = null)
    {
        return new PriceInformation(
            BasePrice: basePrice ?? random.NextDecimal(),
            DiscountPercentByCount: discountPercentByCount ?? GetSome(() => (Key: random.Next(), Value: random.NextDecimal())).ToImmutableDictionary(x => x.Key, x => x.Value));
    }

    public IEnumerable<T> GetSome<T>(Func<T> generator)
    {
        return Enumerable.Range(1, random.Next(0, GetZeroBiasedCount())).Select(_ => generator());
    }
}

internal class NullObject
{
    public static readonly NullObject Instance = new();

    private NullObject()
    {
    }
}

internal enum Option
{
    AutoGenerated = 1,
    Null = 2,
}

internal struct OptionalRef<T> where T : class
{
    private readonly T? value;

    private readonly InternalOption type = InternalOption.AutoGenerated;

    public OptionalRef(T value)
    {
        this.value = value;
        this.type = InternalOption.SpecifiedValue;
    }

    public OptionalRef(Option opt)
    {
        this.value = default;
        this.type = opt switch
        {
            Option.Null => InternalOption.Null,
            Option.AutoGenerated => InternalOption.AutoGenerated,
            _ => throw new NotImplementedException($"Can not handle {opt}"),
        };
    }

    public static implicit operator OptionalRef<T>(T value)
    {
        return new OptionalRef<T>(value);
    }

    public static implicit operator OptionalRef<T>(Option type)
    {
        return new OptionalRef<T>(type);
    }

    public T? Unwrap(Func<T> whenAutoGenerated)
    {
        return type switch
        {
            InternalOption.SpecifiedValue => value,
            InternalOption.Null => null,
            InternalOption.AutoGenerated => whenAutoGenerated(),
            _ => throw new InvalidOperationException(),
        };
    }

    private enum InternalOption
    {
        AutoGenerated = 0,
        SpecifiedValue = 1,
        Null = 2,
    }
}

internal struct OptionalValue<T> where T : struct
{
    private readonly T? value;

    private readonly InternalOption type = InternalOption.AutoGenerated;

    public OptionalValue(T value)
    {
        this.value = value;
        this.type = InternalOption.SpecifiedValue;
    }

    public OptionalValue(Option opt)
    {
        this.value = default;
        this.type = opt switch
        {
            Option.Null => InternalOption.Null,
            Option.AutoGenerated => InternalOption.AutoGenerated,
            _ => throw new NotImplementedException($"Can not handle {opt}"),
        };
    }

    public static implicit operator OptionalValue<T>(T value)
    {
        return new OptionalValue<T>(value);
    }

    public static implicit operator OptionalValue<T>(Option type)
    {
        return new OptionalValue<T>(type);
    }

    public T? Unwrap(Func<T> whenAutoGenerated)
    {
        return type switch
        {
            InternalOption.SpecifiedValue => value,
            InternalOption.Null => null,
            InternalOption.AutoGenerated => whenAutoGenerated(),
            _ => throw new InvalidOperationException(),
        };
    }

    private enum InternalOption
    {
        AutoGenerated = 0,
        SpecifiedValue = 1,
        Null = 2,
    }
}

internal static class RandomExtensions
{
    public static string NextString(this Random r, string? prefix = null) => $"{prefix ?? "RandomString"}_{r.Next(1, 1_000_000)}";

    public static byte[] NextBytes(this Random r, int length = 10)
    {
        byte[] bytes = new byte[16];
        r.NextBytes(bytes);
        return bytes;
    }

    public static Guid NextGuid(this Random r) => new Guid(r.NextBytes(16));

    public static DateTimeOffset NextDateTimeOffset(this Random r) =>  new DateTimeOffset(r.NextInt64(), TimeSpan.FromHours(r.Next(-23, 23)));

    public static TimeSpan NextTimeSpan(this Random r) => new TimeSpan(r.NextInt64());

    public static DateTime NextDateTime(this Random r) => new DateTime(r.Next(2000, 2100), r.Next(1, 13), r.Next(1, 28), r.Next(0, 23), r.Next(0, 59), r.Next(0, 59));

    public static bool NextBool(this Random r) => (r.Next(0, 1000) % 2) == 0;

    public static long NextLong(this Random r) => r.NextInt64();

    public static decimal NextDecimal(this Random r) => (decimal)r.NextDouble();

    public static T NextEnum<T>(this Random r) => Enum.GetValues(typeof(T)).Cast<T>().MinBy(x => r.NextGuid());
}