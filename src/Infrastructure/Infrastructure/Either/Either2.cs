namespace Infrastructure.Either;

public class Either<TA, TB>
{
    private readonly TA? _ta;
    private readonly TB? _tb;
    private readonly Discriminant _discriminant;

    private Either(TA ta)
    {
        _ta = ta;
        _discriminant = Discriminant.A;
    }

    private Either(TB tb)
    {
        _tb = tb;
        _discriminant = Discriminant.B;
    }

    public bool IsA => _discriminant == Discriminant.A;

    public bool IsB => _discriminant == Discriminant.B;

    internal static Either<TA, TB> From(TA ta)
    {
        if (ta == null)
        {
            throw new ArgumentNullException(nameof(ta));
        }

        return new Either<TA, TB>(ta);
    }

    internal static Either<TA, TB> From(TB tb)
    {
        if (tb == null)
        {
            throw new ArgumentNullException(nameof(tb));
        }

        return new Either<TA, TB>(tb);
    }

    public T Switch<T>(Func<TA, T> whenA, Func<TB, T> whenB)
    {
        if (_discriminant == Discriminant.A)
        {
            return whenA(_ta);
        }

        if (_discriminant == Discriminant.B)
        {
            return whenB(_tb);
        }

        throw new InvalidOperationException($"All contained objects are null. This should never be possible.");
    }

    public void Switch(Action<TA> whenA, Action<TB> whenB)
    {
        if (_discriminant == Discriminant.A)
        {
            whenA(_ta);
            return;
        }

        if (_discriminant == Discriminant.B)
        {
            whenB(_tb);
            return;
        }

        throw new InvalidOperationException($"All contained objects are null. This should never be possible.");
    }

    public static implicit operator Either<TA, TB>(TA ta) => From(ta);

    public static implicit operator Either<TA, TB>(TB tb) => From(tb);

    private enum Discriminant
    {
        A = 1,
        B = 2,
    }
}