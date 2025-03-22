﻿namespace TestDataFactoryGenerator.Definitions;

public struct OptionalRef<T> where T : class
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
}