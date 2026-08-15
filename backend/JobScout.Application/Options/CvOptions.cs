namespace JobScout.Application.Options;

public sealed class CvOptions
{
    public const string SectionName = "Cv";

    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    public string[] AllowedExtensions { get; set; } = [".pdf"];
    public string[] AllowedContentTypes { get; set; } = ["application/pdf"];
}
