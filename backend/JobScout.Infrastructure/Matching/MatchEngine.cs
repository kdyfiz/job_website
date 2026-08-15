using JobScout.Application.Interfaces;
using JobScout.Application.Options;
using JobScout.Domain.Enums;
using JobScout.Domain.Models;
using Microsoft.Extensions.Options;

namespace JobScout.Infrastructure.Matching;

public sealed class MatchEngine : IMatchEngine
{
    private readonly MatchingOptions _options;

    public MatchEngine(IOptions<MatchingOptions> options)
    {
        _options = options.Value;
    }

    public JobMatch Score(CandidateProfile profile, Job job, string? query, string? location)
    {
        var cvSkills = new HashSet<string>(profile.Skills, StringComparer.OrdinalIgnoreCase);
        var jobSkills = job.Skills
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var matching = jobSkills.Where(cvSkills.Contains).ToList();
        var missing = jobSkills.Where(s => !cvSkills.Contains(s)).ToList();

        double skillsScore;
        if (jobSkills.Count == 0)
        {
            skillsScore = 0.5;
        }
        else if (cvSkills.Count == 0)
        {
            skillsScore = 0;
        }
        else
        {
            skillsScore = (double)matching.Count / jobSkills.Count;
        }

        var (experienceScore, experienceExplanation) = ScoreExperience(profile, job);
        var locationScore = ScoreLocation(job, location);
        var keywordScore = ScoreKeywords(job, query);

        var weighted =
            (_options.SkillsWeight * skillsScore) +
            (_options.ExperienceWeight * experienceScore) +
            (_options.LocationWeight * locationScore) +
            (_options.KeywordWeight * keywordScore);

        var percent = (int)Math.Round(Math.Clamp(weighted, 0, 1) * 100, MidpointRounding.AwayFromZero);

        return new JobMatch
        {
            Job = job,
            EstimatedMatchPercent = percent,
            Breakdown = new MatchBreakdown
            {
                SkillsScore = skillsScore,
                ExperienceScore = experienceScore,
                LocationScore = locationScore,
                KeywordScore = keywordScore,
                MatchingSkills = matching,
                MissingSkills = missing,
                ExperienceExplanation = experienceExplanation
            }
        };
    }

    private static (double Score, string? Explanation) ScoreExperience(CandidateProfile profile, Job job)
    {
        if (job.ExperienceLevel is null || job.ExperienceLevel == ExperienceLevel.Any)
        {
            return (0.5, null);
        }

        var indicators = string.Join(" ", profile.ExperienceIndicators);
        var detected = DetectCandidateLevel(indicators);

        if (detected is null)
        {
            return (0.5, null);
        }

        var jobLevel = job.ExperienceLevel.Value;
        if (detected == jobLevel)
        {
            return (1.0, ExplanationFor(jobLevel));
        }

        if (Math.Abs((int)detected.Value - (int)jobLevel) == 1)
        {
            return (0.6, ExplanationFor(jobLevel));
        }

        return (0.15, ExplanationFor(jobLevel));
    }

    private static ExperienceLevel? DetectCandidateLevel(string indicators)
    {
        if (string.IsNullOrWhiteSpace(indicators))
        {
            return null;
        }

        if (ContainsAny(indicators, "3 years", "4 years", "5 years"))
        {
            return ExperienceLevel.ThreeToFiveYears;
        }

        if (ContainsAny(indicators, "2 years", "2-year", "1 year", "1-year"))
        {
            return ExperienceLevel.OneToTwoYears;
        }

        if (ContainsAny(indicators, "internship", "intern"))
        {
            return ExperienceLevel.Internship;
        }

        if (ContainsAny(indicators, "fresh graduate", "fresh grad", "entry level", "entry-level", "junior"))
        {
            return ExperienceLevel.FreshGraduate;
        }

        return null;
    }

    private static string? ExplanationFor(ExperienceLevel level)
    {
        return level switch
        {
            ExperienceLevel.Internship => "Appears suitable for internship candidates.",
            ExperienceLevel.FreshGraduate => "Appears suitable for fresh graduate candidates.",
            ExperienceLevel.EntryLevel => "Appears suitable for entry-level candidates.",
            ExperienceLevel.OneToTwoYears => "Listing indicates around 1–2 years of experience.",
            ExperienceLevel.ThreeToFiveYears => "Listing indicates around 3–5 years of experience.",
            _ => null
        };
    }

    private static double ScoreLocation(Job job, string? location)
    {
        if (job.WorkArrangement == WorkArrangement.Remote)
        {
            return 1.0;
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            return 0.5;
        }

        var requested = location.Trim();
        if (job.Location.Contains(requested, StringComparison.OrdinalIgnoreCase) ||
            requested.Contains(job.Location, StringComparison.OrdinalIgnoreCase))
        {
            return 1.0;
        }

        if (ContainsAny(job.Location, "malaysia") && ContainsAny(requested, "malaysia"))
        {
            return 0.8;
        }

        return 0.2;
    }

    private static double ScoreKeywords(Job job, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return 0.5;
        }

        var tokens = query
            .Split([' ', ',', '/', '-'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(t => t.Length > 1)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (tokens.Count == 0)
        {
            return 0.5;
        }

        var haystack = $"{job.Title} {job.Description} {string.Join(' ', job.Skills)}";
        var hits = tokens.Count(token => haystack.Contains(token, StringComparison.OrdinalIgnoreCase));
        return (double)hits / tokens.Count;
    }

    private static bool ContainsAny(string text, params string[] values)
    {
        return values.Any(value => text.Contains(value, StringComparison.OrdinalIgnoreCase));
    }
}
