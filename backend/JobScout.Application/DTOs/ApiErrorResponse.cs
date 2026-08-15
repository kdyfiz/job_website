namespace JobScout.Application.DTOs;

public sealed class ApiErrorResponse
{
    public required ApiError Error { get; init; }
}

public sealed class ApiError
{
    public required string Code { get; init; }
    public required string Message { get; init; }
}
