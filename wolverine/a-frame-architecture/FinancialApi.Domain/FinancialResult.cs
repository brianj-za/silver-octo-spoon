namespace FinancialApi.Domain;

public interface IFinancialError
{
    int ErrorCode { get; }
    string Message { get; }
    Exception? Exception { get; }
}

public sealed class FinancialError : IFinancialError
{
    public FinancialError(int errorCode, string message, Exception? exception = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(errorCode);

        ErrorCode = errorCode;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Exception = exception;
    }

    public int ErrorCode { get; }

    public string Message { get; }

    public Exception? Exception { get; }

    public override string ToString() => $"Error: {ErrorCode} - {Message}{(Exception == null ? "" : $"\n{Exception}")}";
}

public sealed class FinancialResult<T>
{
    private readonly T? _resultValue;
    private readonly FinancialError? _resultError;

    public FinancialResult(T resultValue)
    {
        IsError = false;
        _resultValue = resultValue;
        _resultError = null;
    }

    public FinancialResult(FinancialError resultError)
    {
        IsError = true;
        _resultValue = default;
        _resultError = resultError;
    }

    public bool IsError { get; }

    /// <summary>
    /// Error indicator
    /// </summary>
    public bool IsSuccess => !IsError;

    /// <summary>
    /// Result Values
    /// </summary>
    public T? Value => _resultValue;

    public FinancialError? Error => _resultError;

    public static implicit operator FinancialResult<T>(T resultValue) => new(resultValue);

    public static implicit operator FinancialResult<T>(FinancialError resultError) => new(resultError);

    public static FinancialResult<T> Success(T successValue)
    {
        return new FinancialResult<T>(successValue);
    }

    public static FinancialResult<T> Failure(FinancialError failureValue)
    {
        return new FinancialResult<T>(failureValue);
    }

    public TResult Match<TResult>(
        Func<T, TResult> success,
        Func<FinancialError, TResult> failure,
        Func<TResult> nullValue
    )
    {
        return IsError switch
        {
            false when _resultValue == null => nullValue(),
            true => failure(_resultError!),
            _ => success(_resultValue)
        };
    }

    public TResult Match<TResult>(Func<T, TResult> success, Func<FinancialError, TResult> failure)
    {
        return IsError switch
        {
            true => failure(_resultError!),
            _ => success(_resultValue!)
        };
    }

    public TResult? Match<TResult>(Func<T, TResult> success)
    {
        return IsError switch
        {
            true => default,
            _ => success(_resultValue!)
        };
    }

    public void Match(Action<T> success, Action<FinancialError> failure, Action nullValue)
    {
        switch (IsError)
        {
            case false when _resultValue == null:
                nullValue.Invoke();
                return;
            case true:
                failure.Invoke(_resultError!);
                return;
            default:
                success(_resultValue);
                break;
        }
    }

    public void Match(Action<T> success, Action<FinancialError> failure)
    {
        switch (IsError)
        {
            case true:
                failure.Invoke(_resultError!);
                return;
            default:
                success(_resultValue!);
                break;
        }
    }

    public void Match(Action<T> success)
    {
        if (IsError)
        {
            return;
        }

        success(_resultValue!);
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> success,
        Func<FinancialError, Task<TResult>> failure,
        Func<Task<TResult>> nullValue
    )
    {
        return IsError switch
        {
            false when _resultValue == null => await nullValue(),
            true => await failure(_resultError!),
            _ => await success(_resultValue)
        };
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> success,
        Func<FinancialError, Task<TResult>> failure
    )
    {
        return IsError switch
        {
            true => await failure(_resultError!),
            _ => await success(_resultValue!)
        };
    }

    public async Task<TResult?> MatchAsync<TResult>(Func<T, Task<TResult>> success)
    {
        return IsError switch
        {
            true => default,
            _ => await success(_resultValue!)
        };
    }

    public async Task MatchAsync(Func<T, Task> success, Func<FinancialError, Task> failure, Func<Task> nullValue)
    {
        switch (IsError)
        {
            case false when _resultValue == null:
                await nullValue.Invoke();
                return;
            case true:
                await failure.Invoke(_resultError!);
                return;
            default:
                await success(_resultValue!);
                break;
        }
    }

    public async Task MatchAsync(Func<T, Task> success, Func<FinancialError, Task> failure)
    {
        switch (IsError)
        {
            case true:
                await failure.Invoke(_resultError!);
                return;
            default:
                await success(_resultValue!);
                break;
        }
    }

    public async Task MatchAsync(Func<T, Task> success)
    {
        if (IsError)
        {
            return;
        }

        await success(_resultValue!);
    }
}