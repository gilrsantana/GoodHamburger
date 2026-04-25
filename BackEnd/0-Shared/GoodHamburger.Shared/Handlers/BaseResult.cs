namespace GoodHamburger.Shared.Handlers;

public class BaseResult
{
    public bool IsSuccess { get; }
    public Error Error { get; }
    public bool IsFailure => !IsSuccess;

    protected BaseResult(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static BaseResult Success() => new(true, Error.None);
    public static BaseResult Failure(Error error) => new(false, error);
}