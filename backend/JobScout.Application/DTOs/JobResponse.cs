using JobScout.Domain.Enums;
using JobScout.Domain.Models;

namespace JobScout.Application.DTOs;

public sealed class JobResponse
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
    public AvailabilityStatus AvailabilityStatus { get; init; }
    public bool IsDemoData { get; init; }
    public int? EstimatedMatchPercent { get; init; }
    public MatchExplanationResponse? Match { get; init; }
}

public sealed class MatchExplanationResponse
{
    public int EstimatedMatchPercent { get; init; }
    public IReadOnlyList<string> MatchingSkills { get; init; } = [];
    public IReadOnlyList<string> MissingSkills { get; init; } = [];
    public string? ExperienceExplanation { get; init; }
    public string Disclaimer { get; init; } =
        "Match scores are estimates based on information detected from your CV and the job listing. They are not a guarantee of suitability or employment.";
}
