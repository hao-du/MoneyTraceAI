namespace MoneyTrace.Domain.Primitives;

public class Error
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not Error error)
        {
            return false;
        }

        return error.Code == Code && error.Message == Message;
    }

    public override int GetHashCode() => Code.GetHashCode() * 41 + Message.GetHashCode();
}
