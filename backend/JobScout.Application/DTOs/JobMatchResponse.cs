namespace JobScout.Application.DTOs;

public sealed class JobMatchResponse
{
    public required CVAnalysisResponse Cv { get; init; }
    public required JobSearchResponse Results { get; init; }
}
