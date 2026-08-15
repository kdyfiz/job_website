using JobScout.Domain.Enums;

namespace JobScout.Domain.Models;

public sealed record Job
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Company { get; init; }
    public required string Location { get; init; }
    public required string Description { get; init; }
    public EmploymentType? EmploymentType { get; init; }
    public ExperienceLevel? ExperienceLevel { get; init; }
    public WorkArrangement? WorkArrangement { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = [];
    public SalaryInfo? Salary { get; init; }
    public DateTimeOffset? PostedDate { get; init; }
    public required string Source { get; init; }
    public string? SourceUrl { get; init; }
    public AvailabilityStatus AvailabilityStatus { get; init; } = AvailabilityStatus.AvailabilityUnknown;
}
