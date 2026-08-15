namespace JobScout.Domain.Models;

public sealed class CandidateProfile
{
    public IReadOnlyList<string> Skills { get; init; } = [];
    public IReadOnlyList<string> ExperienceIndicators { get; init; } = [];
    public string? ExtractedTextPreview { get; init; }
}
