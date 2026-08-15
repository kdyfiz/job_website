using JobScout.Application.Options;
using JobScout.Domain.Enums;
using JobScout.Domain.Models;
using JobScout.Infrastructure.Matching;
using Microsoft.Extensions.Options;

namespace JobScout.UnitTests;

public class MatchEngineTests
{
    private readonly MatchEngine _engine = new(Options.Create(new MatchingOptions()));

    [Fact]
    public void Full_skill_match_scores_high()
    {
        var result = _engine.Score(
            Profile(["Java", "SQL", "React"]),
            JobWithSkills(["Java", "SQL", "React"]),
            "Java developer",
            "Kuala Lumpur");

        Assert.True(result.EstimatedMatchPercent >= 80);
        Assert.Equal(["Java", "SQL", "React"], result.Breakdown.MatchingSkills);
        Assert.Empty(result.Breakdown.MissingSkills);
    }

    [Fact]
    public void Partial_skill_match_lists_gaps()
    {
        var result = _engine.Score(
            Profile(["Java", "SQL", "React"]),
            JobWithSkills(["Java", "SQL", "React", "Docker", "REST API"]),
            null,
            null);

        Assert.Contains("Java", result.Breakdown.MatchingSkills);
        Assert.Contains("Docker", result.Breakdown.MissingSkills);
        Assert.Contains("REST API", result.Breakdown.MissingSkills);
        Assert.InRange(result.EstimatedMatchPercent, 30, 80);
    }

    [Fact]
    public void No_skill_match_returns_zero_skill_component()
    {
        var result = _engine.Score(
            Profile(["Python"]),
            JobWithSkills(["Java", "SQL"]),
            null,
            null);

        Assert.Empty(result.Breakdown.MatchingSkills);
        Assert.Equal(0, result.Breakdown.SkillsScore);
        Assert.True(result.EstimatedMatchPercent < 50);
    }

    [Fact]
    public void Empty_cv_skills_do_not_invent_matches()
    {
        var result = _engine.Score(
            Profile([]),
            JobWithSkills(["Java"]),
            null,
            null);

        Assert.Empty(result.Breakdown.MatchingSkills);
        Assert.Equal(["Java"], result.Breakdown.MissingSkills);
        Assert.Equal(0, result.Breakdown.SkillsScore);
    }

    [Fact]
    public void Job_without_skills_uses_neutral_skill_score()
    {
        var result = _engine.Score(
            Profile(["Java"]),
            JobWithSkills([]),
            null,
            null);

        Assert.Equal(0.5, result.Breakdown.SkillsScore);
    }

    [Fact]
    public void Fresh_graduate_matches_fresh_graduate_role()
    {
        var profile = new CandidateProfile
        {
            Skills = ["Java"],
            ExperienceIndicators = ["Fresh Graduate"]
        };

        var job = JobWithSkills(["Java"]) with { ExperienceLevel = ExperienceLevel.FreshGraduate };
        var result = _engine.Score(profile, job, null, null);

        Assert.Equal(1.0, result.Breakdown.ExperienceScore);
        Assert.Contains("fresh graduate", result.Breakdown.ExperienceExplanation, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Score_is_reproducible()
    {
        var profile = Profile(["Java", "SQL"]);
        var job = JobWithSkills(["Java", "SQL", "Docker"]);
        var first = _engine.Score(profile, job, "developer", "Malaysia");
        var second = _engine.Score(profile, job, "developer", "Malaysia");
        Assert.Equal(first.EstimatedMatchPercent, second.EstimatedMatchPercent);
    }

    private static CandidateProfile Profile(string[] skills) => new() { Skills = skills };

    private static Job JobWithSkills(string[] skills) => new()
    {
        Id = "demo-test",
        Title = "Junior Software Developer",
        Company = "Test Co",
        Location = "Kuala Lumpur, Malaysia",
        Description = "Java SQL React developer role in Kuala Lumpur",
        Skills = skills,
        Source = "Demo",
        ExperienceLevel = ExperienceLevel.EntryLevel,
        WorkArrangement = WorkArrangement.Hybrid
    };
}
