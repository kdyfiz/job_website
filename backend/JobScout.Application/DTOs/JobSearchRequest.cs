using JobScout.Domain.Enums;

namespace JobScout.Application.DTOs;

public sealed class JobSearchRequest
{
    public string? Query { get; init; }
    public string? Location { get; init; }

    public string EffectiveLocation =>
        string.IsNullOrWhiteSpace(Location) ? "Malaysia" : Location.Trim();

    public ExperienceLevel ExperienceLevel { get; init; } = ExperienceLevel.Any;
    public WorkArrangement WorkArrangement { get; init; } = WorkArrangement.Any;
    public EmploymentType EmploymentType { get; init; } = EmploymentType.Any;
    public DatePostedFilter DatePosted { get; init; } = DatePostedFilter.Any;
    public JobSortOption Sort { get; init; } = JobSortOption.MostRelevant;
    public MatchScoreFilter MinMatchScore { get; init; } = MatchScoreFilter.Any;
    public bool QueryRequired { get; init; } = true;
}
