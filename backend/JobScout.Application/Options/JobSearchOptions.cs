namespace JobScout.Application.Options;

public sealed class JobSearchOptions
{
    public const string SectionName = "JobSearch";

    public bool UseLiveListings { get; set; } = true;
}
