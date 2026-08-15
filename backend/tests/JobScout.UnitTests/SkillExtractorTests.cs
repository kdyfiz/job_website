using JobScout.Infrastructure.CV;

namespace JobScout.UnitTests;

public class SkillExtractorTests
{
    private readonly SkillExtractor _extractor = new(SkillCatalog.LoadFromEmbeddedResource());

    [Fact]
    public void Detects_configured_skills_and_aliases()
    {
        var profile = _extractor.Extract("Worked with Java, SQL, React.js, Postman and C# on ASP.NET Core.");

        Assert.Contains("Java", profile.Skills);
        Assert.Contains("SQL", profile.Skills);
        Assert.Contains("React", profile.Skills);
        Assert.Contains("Postman", profile.Skills);
        Assert.Contains("C#", profile.Skills);
        Assert.Contains("ASP.NET Core", profile.Skills);
    }

    [Fact]
    public void Detects_experience_phrases_without_inventing_others()
    {
        var profile = _extractor.Extract("Fresh Graduate. Internship as QA Analyst. Software Developer.");

        Assert.Contains("Fresh Graduate", profile.ExperienceIndicators);
        Assert.Contains("Internship", profile.ExperienceIndicators);
        Assert.Contains("QA Analyst", profile.ExperienceIndicators);
        Assert.DoesNotContain("5 years", profile.ExperienceIndicators);
    }

    [Fact]
    public void Empty_text_returns_no_skills()
    {
        var profile = _extractor.Extract("   ");
        Assert.Empty(profile.Skills);
        Assert.Empty(profile.ExperienceIndicators);
    }
}
