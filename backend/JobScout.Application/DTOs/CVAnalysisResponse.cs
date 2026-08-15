namespace JobScout.Application.DTOs;

public sealed class CVAnalysisResponse
{
    public int SkillCount { get; init; }
    public IReadOnlyList<string> Skills { get; init; } = [];
    public IReadOnlyList<string> ExperienceIndicators { get; init; } = [];
    public string? Warning { get; init; }
}
