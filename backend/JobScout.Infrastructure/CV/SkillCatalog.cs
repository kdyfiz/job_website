using System.Text.Json;

namespace JobScout.Infrastructure.CV;

public sealed class SkillCatalog
{
    public IReadOnlyList<string> Skills { get; }
    public IReadOnlyDictionary<string, string> Aliases { get; }
    public IReadOnlyList<string> ExperiencePhrases { get; }

    public SkillCatalog(IEnumerable<string> skills, IReadOnlyDictionary<string, string> aliases, IEnumerable<string> experiencePhrases)
    {
        Skills = skills
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(s => s.Length)
            .ToList();
        Aliases = aliases;
        ExperiencePhrases = experiencePhrases
            .OrderByDescending(p => p.Length)
            .ToList();
    }

    public static SkillCatalog LoadFromEmbeddedResource()
    {
        var assembly = typeof(SkillCatalog).Assembly;
        var name = assembly.GetManifestResourceNames()
            .First(n => n.EndsWith("skills.json", StringComparison.OrdinalIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException("skills.json embedded resource was not found.");
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var document = JsonSerializer.Deserialize<SkillCatalogFile>(json, JsonOptions)
            ?? throw new InvalidOperationException("skills.json could not be parsed.");

        var skills = document.Categories.Values.SelectMany(v => v);
        return new SkillCatalog(skills, document.Aliases, document.ExperiencePhrases);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private sealed class SkillCatalogFile
    {
        public Dictionary<string, List<string>> Categories { get; set; } = [];
        public Dictionary<string, string> Aliases { get; set; } = [];
        public List<string> ExperiencePhrases { get; set; } = [];
    }
}
