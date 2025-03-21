namespace Infrastructure;

public class GenericError
{
    public GenericError(string errorKey, string errorValue)
    {
        Errors = new[] { (errorKey, errorValue) };
    }

    public GenericError(IReadOnlyList<(string errorKey, string errorValue)> errors)
    {
        Errors = errors;
    }

    protected GenericError()
    {
    }

    public IReadOnlyList<(string errorKey, string errorValue)> Errors { get; protected set; }

    public override string ToString()
    {
        return string.Join("\n", Errors?.Select(e => $"{e.errorKey} {e.errorValue}") ?? Array.Empty<string>());
    }
}

public class GenericError<T> : GenericError
{
    public GenericError(T error)
    {
        WrappedError = error;
        Errors = new[] { ("Exception", error.ToString()) };
    }

    public T WrappedError { get; }
}