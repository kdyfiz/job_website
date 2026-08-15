namespace JobScout.Application.Exceptions;

public sealed class JobScoutException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public JobScoutException(string code, string message, int statusCode = 400)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}
