using System;

namespace BuilderGenerator.Sources.Reflection;

internal enum NullabilityContextKind
{
    Standard = 1,
    Inverted = 2,
    None = 3,
}

internal static class NullabilityContextKindExtensions
{
    public static T Switch<T>(
        this NullabilityContextKind kind,
        Func<T> whenStandard,
        Func<T> whenInverted,
        Func<T> whenNone)
    {
        return kind switch
        {
            NullabilityContextKind.Standard => whenStandard(),
            NullabilityContextKind.Inverted => whenInverted(),
            NullabilityContextKind.None => whenNone(),
            _ => throw new InvalidOperationException(
                $"Can not handle {nameof(NullabilityContextKind)} with value {kind}"),
        };
    }
}