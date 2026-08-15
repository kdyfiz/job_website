namespace JobScout.Application.DTOs;

public sealed class JobSearchResponse
{
    public int Total { get; init; }
    public required string Query { get; init; }
    public string? Location { get; init; }
    public bool UsingDemoData { get; init; }
    public IReadOnlyList<JobResponse> Jobs { get; init; } = [];
}
