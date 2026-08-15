namespace JobScout.Application.Options;

public sealed class MatchingOptions
{
    public const string SectionName = "Matching";

    public double SkillsWeight { get; set; } = 0.6;
    public double ExperienceWeight { get; set; } = 0.2;
    public double LocationWeight { get; set; } = 0.1;
    public double KeywordWeight { get; set; } = 0.1;
}
