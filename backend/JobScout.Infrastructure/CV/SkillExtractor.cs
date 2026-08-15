using System.Text.RegularExpressions;
using JobScout.Application.Interfaces;
using JobScout.Domain.Models;

namespace JobScout.Infrastructure.CV;

public sealed class SkillExtractor : ISkillExtractor
{
    private readonly SkillCatalog _catalog;

    public SkillExtractor(SkillCatalog catalog)
    {
        _catalog = catalog;
    }

    public CandidateProfile Extract(string cvText)
    {
        var normalized = cvText ?? string.Empty;
        var found = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var alias in _catalog.Aliases)
        {
            if (ContainsToken(normalized, alias.Key))
            {
                found.Add(alias.Value);
            }
        }

        foreach (var skill in _catalog.Skills)
        {
            if (ContainsToken(normalized, skill))
            {
                found.Add(skill);
            }
        }

        var indicators = new List<string>();
        foreach (var phrase in _catalog.ExperiencePhrases)
        {
            if (normalized.Contains(phrase, StringComparison.OrdinalIgnoreCase) &&
                !indicators.Any(existing => existing.Equals(phrase, StringComparison.OrdinalIgnoreCase)))
            {
                indicators.Add(phrase);
            }
        }

        return new CandidateProfile
        {
            Skills = found.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList(),
            ExperienceIndicators = indicators,
            ExtractedTextPreview = Truncate(normalized, 400)
        };
    }

    private static bool ContainsToken(string text, string skill)
    {
        if (string.IsNullOrWhiteSpace(skill))
        {
            return false;
        }

        var escaped = Regex.Escape(skill);
        var pattern = $@"(?:^|[^\w+#]){escaped}(?:$|[^\w+#])";
        return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
    }

    private static string Truncate(string value, int max)
    {
        var trimmed = value.Trim();
        return trimmed.Length <= max ? trimmed : trimmed[..max] + "…";
    }
}
