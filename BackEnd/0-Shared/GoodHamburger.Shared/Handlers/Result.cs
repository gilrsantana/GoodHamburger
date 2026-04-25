namespace GoodHamburger.Shared.Handlers;

public class Result<TValue> : BaseResult
{
    public TValue? Value { get; }

    protected Result(TValue? value, bool isSuccess, Error error) 
        : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
    public new static Result<TValue> Failure(Error error) => new(default, false, error);
}