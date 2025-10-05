using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


using TTS = TestDataFactoryGenerator.TestData.SharedStuff;
using TTS1 = TestDataFactoryGenerator.TestData.Served;

namespace TestDataFactoryGenerator.TestData;

/// <summary>
/// This class was auto generated using TestDataFactoryGenerator, for following types:<br/>
/// - <see cref="TestDataFactoryGenerator.TestData.Served.Order"/><br/>
/// - <see cref="TestDataFactoryGenerator.TestData.Served.Delivery"/><br/>
/// This class can be edited, but it is preferable to not touch this file, and extend the partial class in a separate file.
/// </summary>
internal partial class Tdf
{
    private static readonly IImmutableList<int> _zeroBiasedCounts = [0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2];

    private int GetZeroBiasedCount() => _zeroBiasedCounts.OrderBy(_ => _random.Next()).First();

    private readonly Random _random;

    public Tdf()
    {
        _random = new Random();
    }

    public Tdf(int seed)
    {
        _random = new Random(seed);
    }

    public Tdf(Random random)
    {
        _random = random;
    }

    public TTS.Address GenerateAddress(
        string? street = null,
        string? city = null)
    {
        return new TTS.Address(
            Street: street ?? _random.NextString(""),
            City: city ?? _random.NextString(""));
    }

    public TTS1.AllCollections GenerateAllCollections(
        IEnumerable<TTS1.PositionNote>? positionNoteIEnumerable = null,
        ICollection<TTS1.PositionNote>? positionNoteICollection = null,
        IReadOnlyCollection<TTS1.PositionNote>? positionNoteIReadonOnlyCollection = null,
        ImmutableArray<TTS1.PositionNote>? positionNoteImmutableArray = null,
        TTS1.PositionNote[]? positionNoteArray = null,
        List<TTS1.PositionNote>? positionNoteList = null,
        LinkedList<TTS1.PositionNote>? positionNoteLinkedList = null,
        ImmutableList<TTS1.PositionNote>? positionNoteImmutableList = null,
        IImmutableList<TTS1.PositionNote>? positionNoteIImmutableList = null,
        IReadOnlyList<TTS1.PositionNote>? positionNoteIReadOnlyList = null,
        ISet<TTS1.PositionNote>? positionNoteSet = null,
        IImmutableSet<TTS1.PositionNote>? positionNoteIImmutableSet = null,
        ImmutableHashSet<TTS1.PositionNote>? positionNoteImmutableHAshSet = null,
        ImmutableSortedSet<TTS1.PositionNote>? positionNoteImmutableSortedSet = null,
        IReadOnlySet<TTS1.PositionNote>? positionNoteIReadOnlySet = null,
        FrozenSet<TTS1.PositionNote>? positionNoteFrozenSet = null,
        SortedSet<TTS1.PositionNote>? positionNoteSortedSet = null,
        IImmutableDictionary<Guid, TTS1.PositionNote>? positionNoteIImmutableDictionary = null,
        ImmutableDictionary<Guid, TTS1.PositionNote>? positionNoteImmutableDictionary = null,
        ImmutableSortedDictionary<Guid, TTS1.PositionNote>? positionNoteImmutableSortedDictionary = null,
        FrozenDictionary<Guid, TTS1.PositionNote>? positionNoteFrozenDictionary = null,
        Dictionary<Guid, TTS1.PositionNote>? positionNoteDictionary = null,
        Queue<TTS1.PositionNote>? positionNoteQueue = null,
        Stack<TTS1.PositionNote>? positionNoteStack = null,
        OptionalRef<FrozenSet<TTS1.PositionNote>> optionalPositionNoteFrozenSet = default)
    {
        return new TTS1.AllCollections(
            PositionNoteIEnumerable: positionNoteIEnumerable ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteICollection: positionNoteICollection ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIReadonOnlyCollection: positionNoteIReadonOnlyCollection ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteImmutableArray: positionNoteImmutableArray ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteArray: positionNoteArray ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteList: positionNoteList ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteLinkedList: positionNoteLinkedList ?? new LinkedList<TTS1.PositionNote>([..GetSome(() => GeneratePositionNote())]),
            PositionNoteImmutableList: positionNoteImmutableList ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIImmutableList: positionNoteIImmutableList ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIReadOnlyList: positionNoteIReadOnlyList ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteSet: positionNoteSet ?? new HashSet<TTS1.PositionNote>([..GetSome(() => GeneratePositionNote())]),
            PositionNoteIImmutableSet: positionNoteIImmutableSet ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteImmutableHAshSet: positionNoteImmutableHAshSet ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteImmutableSortedSet: positionNoteImmutableSortedSet ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIReadOnlySet: positionNoteIReadOnlySet ?? GetSome(() => GeneratePositionNote()).ToHashSet(),
            PositionNoteFrozenSet: positionNoteFrozenSet ?? GetSome(() => GeneratePositionNote()).ToFrozenSet(),
            PositionNoteSortedSet: positionNoteSortedSet ?? [..GetSome(() => GeneratePositionNote())],
            PositionNoteIImmutableDictionary: positionNoteIImmutableDictionary ?? GetSome(() => (Key: _random.NextGuid(), Value: GeneratePositionNote())).ToImmutableDictionary(x => x.Key, x => x.Value),
            PositionNoteImmutableDictionary: positionNoteImmutableDictionary ?? GetSome(() => (Key: _random.NextGuid(), Value: GeneratePositionNote())).ToImmutableDictionary(x => x.Key, x => x.Value),
            PositionNoteImmutableSortedDictionary: positionNoteImmutableSortedDictionary ?? GetSome(() => (Key: _random.NextGuid(), Value: GeneratePositionNote())).ToImmutableSortedDictionary(x => x.Key, x => x.Value),
            PositionNoteFrozenDictionary: positionNoteFrozenDictionary ?? GetSome(() => (Key: _random.NextGuid(), Value: GeneratePositionNote())).ToFrozenDictionary(x => x.Key, x => x.Value),
            PositionNoteDictionary: positionNoteDictionary ?? GetSome(() => (Key: _random.NextGuid(), Value: GeneratePositionNote())).ToDictionary(x => x.Key, x => x.Value),
            PositionNoteQueue: positionNoteQueue ?? new Queue<TTS1.PositionNote>([..GetSome(() => GeneratePositionNote())]),
            PositionNoteStack: positionNoteStack ?? new Stack<TTS1.PositionNote>([..GetSome(() => GeneratePositionNote())]),
            OptionalPositionNoteFrozenSet: optionalPositionNoteFrozenSet.Unwrap(whenAutoGenerated: () => GetSome(() => GeneratePositionNote()).ToFrozenSet()));
    }

    public TTS1.ContactInfo GenerateContactInfo()
    {
        return _random.Next(0, 2) switch {
            0 => GeneratePhoneContactInfo(),
            1 => GenerateEmailContactInfo(),
            _ => throw new InvalidOperationException("Unexpected constructor number"),
        };
    }

    public TTS1.Delivery GenerateDelivery(
        IImmutableList<TTS1.ItemPosition>? items = null,
        DateTimeOffset? deliveryDate = null)
    {
        return new TTS1.Delivery(
            Items: items ?? [..GetSome(() => GenerateItemPosition())],
            DeliveryDate: deliveryDate ?? _random.NextDateTimeOffset());
    }

    public TTS1.EmailContactInfo GenerateEmailContactInfo(
        TTS1.Person? person = null,
        string? email = null)
    {
        return new TTS1.EmailContactInfo(
            Person: person ?? GeneratePerson(),
            Email: email ?? _random.NextString(""));
    }

    public TTS1.ItemPosition GenerateItemPosition(
        Guid? itemPositionId = null,
        Guid? productId = null,
        string? productShortName = null,
        int? itemCount = null,
        decimal? positionPrice = null)
    {
        return new TTS1.ItemPosition(
            ItemPositionId: itemPositionId ?? _random.NextGuid(),
            ProductId: productId ?? _random.NextGuid(),
            ProductShortName: productShortName ?? _random.NextString(""),
            ItemCount: itemCount ?? _random.Next(),
            PositionPrice: positionPrice ?? _random.NextDecimal());
    }

    public TTS1.Order GenerateOrder(
        Guid? orderId = null,
        DateTimeOffset? orderDate = null,
        IImmutableSet<TTS1.Position>? position = null,
        TTS1.ContactInfo? contactInfo = null,
        TTS1.Delivery? delivery = null,
        IImmutableDictionary<Guid, TTS1.PriceInformation>? priceByProductId = null,
        TTS1.AllCollections? allCollections = null)
    {
        return new TTS1.Order(
            OrderId: orderId ?? _random.NextGuid(),
            OrderDate: orderDate ?? _random.NextDateTimeOffset(),
            Position: position ?? [..GetSome(() => GeneratePosition())],
            ContactInfo: contactInfo ?? GenerateContactInfo(),
            Delivery: delivery ?? GenerateDelivery(),
            PriceByProductId: priceByProductId ?? GetSome(() => (Key: _random.NextGuid(), Value: GeneratePriceInformation())).ToImmutableDictionary(x => x.Key, x => x.Value),
            AllCollections: allCollections ?? GenerateAllCollections());
    }

    public TTS1.Person GeneratePerson(
        Guid? id = null,
        string? firstName = null,
        string? lastName = null,
        TTS.Address? address = null)
    {
        return new TTS1.Person(
            Id: id ?? _random.NextGuid(),
            FirstName: firstName ?? _random.NextString(""),
            LastName: lastName ?? _random.NextString(""),
            Address: address ?? GenerateAddress());
    }

    public TTS1.PhoneContactInfo GeneratePhoneContactInfo(
        TTS1.Person? person = null,
        string? phoneNumber = null)
    {
        return new TTS1.PhoneContactInfo(
            Person: person ?? GeneratePerson(),
            PhoneNumber: phoneNumber ?? _random.NextString(""));
    }

    public TTS1.Position GeneratePosition(
        Guid? positionId = null,
        Guid? productId = null,
        int? count = null,
        OptionalRef<TTS1.PositionNote> positionNote = default,
        OptionalValue<int> batchSize = default)
    {
        return new TTS1.Position(
            PositionId: positionId ?? _random.NextGuid(),
            ProductId: productId ?? _random.NextGuid(),
            Count: count ?? _random.Next(),
            PositionNote: positionNote.Unwrap(whenAutoGenerated: () => GeneratePositionNote()),
            BatchSize: batchSize.Unwrap(whenAutoGenerated: () => _random.Next()));
    }

    public TTS1.PositionNote GeneratePositionNote(
        string? note = null)
    {
        return new TTS1.PositionNote(
            Note: note ?? _random.NextString(""));
    }

    public TTS1.PriceInformation GeneratePriceInformation(
        decimal? basePrice = null,
        IImmutableDictionary<int, decimal>? discountPercentByCount = null)
    {
        return new TTS1.PriceInformation(
            BasePrice: basePrice ?? _random.NextDecimal(),
            DiscountPercentByCount: discountPercentByCount ?? GetSome(() => (Key: _random.Next(), Value: _random.NextDecimal())).ToImmutableDictionary(x => x.Key, x => x.Value));
    }

    public IEnumerable<T> GetSome<T>(Func<T> generator)
    {
        return Enumerable.Range(1, _random.Next(0, GetZeroBiasedCount())).Select(_ => generator());
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
    private readonly T? _value;

    private readonly InternalOption _type = InternalOption.AutoGenerated;

    public OptionalRef(T value)
    {
        _value = value;
        _type = InternalOption.SpecifiedValue;
    }

    public OptionalRef(Option opt)
    {
        _value = default;
        _type = opt switch
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
        return _type switch
        {
            InternalOption.SpecifiedValue => _value,
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
    private readonly T? _value;

    private readonly InternalOption _type = InternalOption.AutoGenerated;

    public OptionalValue(T value)
    {
        _value = value;
        _type = InternalOption.SpecifiedValue;
    }

    public OptionalValue(Option opt)
    {
        _value = default;
        _type = opt switch
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
        return _type switch
        {
            InternalOption.SpecifiedValue => _value,
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
    public static string NextString(this Random r, string? prefix) => $"{prefix ?? "RandomString"}_{r.Next(1, 1_000_000)}";

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
}