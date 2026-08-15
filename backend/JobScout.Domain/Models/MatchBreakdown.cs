namespace JobScout.Domain.Models;

public sealed class MatchBreakdown
{
    public double SkillsScore { get; init; }
    public double ExperienceScore { get; init; }
    public double LocationScore { get; init; }
    public double KeywordScore { get; init; }
    public IReadOnlyList<string> MatchingSkills { get; init; } = [];
    public IReadOnlyList<string> MissingSkills { get; init; } = [];
    public string? ExperienceExplanation { get; init; }
}
