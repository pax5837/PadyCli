using Infrastructure.Either;

namespace Infrastructure.Piping;

public static class PipingExtensions
{
    public static Either<T, GenericError> Pipe<T>(
        this T input,
        Action action,
        Func<Exception, GenericError>? errorHandler = null)
    {
        try
        {
            action.Invoke();
            return input;
        }
        catch (Exception e)
        {
            return errorHandler?.Invoke(e) ?? new GenericError<Exception>(e);
        }
    }

    public static Either<T, GenericError> Pipe<T>(
        this T input,
        Action<T> action,
        Func<Exception, GenericError>? errorHandler = null)
    {
        try
        {
            action(input);
            return input;
        }
        catch (Exception e)
        {
            return errorHandler?.Invoke(e) ?? new GenericError<Exception>(e);
        }
    }

    public static Either<TOut, GenericError> Pipe<TIn, TOut>(
        this TIn input,
        Func<TIn, TOut> func,
        Func<Exception, GenericError>? errorHandler = null)
    {
        try
        {
            return func(input);
        }
        catch (Exception e)
        {
            return errorHandler?.Invoke(e) ?? new GenericError<Exception>(e);
        }
    }

    public static Either<TOut, GenericError> Pipe<TIn, TOut>(
        this TIn input,
        Func<TIn, Either<TOut, GenericError>> func,
        Func<Exception, GenericError>? errorHandler = null)
    {
        try
        {
            return func(input);
        }
        catch (Exception e)
        {
            return errorHandler?.Invoke(e) ?? new GenericError<Exception>(e);
        }
    }

    public static Either<T, GenericError> Pipe<T>(
        this Either<T, GenericError> input,
        Action action,
        Func<Exception, GenericError>? errorHandler = null)
    {
        return input.Switch(
            x => x.Pipe(action, errorHandler),
            e => e);
    }

    public static Either<T, GenericError> Pipe<T>(
        this Either<T, GenericError> input,
        Action<T> action,
        Func<Exception, GenericError>? errorHandler = null)
    {
        return input.Switch(
            x => x.Pipe(action, errorHandler),
            e => e);
    }

    public static Either<T, GenericError> Pipe<T>(
        this Either<T, GenericError> input,
        Action<GenericError> action)
    {
        input.Switch(
            _ => { },
            error => action(error));

        return input;
    }

    public static Either<TOut, GenericError> Pipe<TIn, TOut>(
        this Either<TIn, GenericError> input,
        Func<TIn, TOut> func,
        Func<Exception, GenericError>? errorHandler = null)
    {
        return input.Switch(
            x => x.Pipe(func, errorHandler),
            e => e);
    }

    public static Either<TOut, GenericError> Pipe<TIn, TOut>(
        this Either<TIn, GenericError> input,
        Func<TIn, Either<TOut, GenericError>> func,
        Func<Exception, GenericError>? errorHandler = null)
    {
        return input.Switch(
            x =>
            {
                try
                {
                    return func(x);
                }
                catch (Exception e)
                {
                    return errorHandler?.Invoke(e) ?? new GenericError<Exception>(e);
                }
            },
            e => e);
    }
}