using JobScout.Application.DTOs;
using JobScout.Application.Interfaces;
using JobScout.Application.Mapping;
using JobScout.Domain.Enums;
using JobScout.Domain.Models;

namespace JobScout.Application.Services;

public sealed class JobMatchService : IJobMatchService
{
    private readonly ICvAnalysisService _cvAnalysis;
    private readonly ISkillExtractor _skillExtractor;
    private readonly IPdfTextExtractor _extractor;
    private readonly IJobSearchProvider _provider;
    private readonly IMatchEngine _matchEngine;

    public JobMatchService(
        ICvAnalysisService cvAnalysis,
        ISkillExtractor skillExtractor,
        IPdfTextExtractor extractor,
        IJobSearchProvider provider,
        IMatchEngine matchEngine)
    {
        _cvAnalysis = cvAnalysis;
        _skillExtractor = skillExtractor;
        _extractor = extractor;
        _provider = provider;
        _matchEngine = matchEngine;
    }

    public async Task<JobMatchResponse> MatchAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long length,
        JobSearchRequest searchRequest,
        CancellationToken cancellationToken = default)
    {
        await using var copy = new MemoryStream();
        await fileStream.CopyToAsync(copy, cancellationToken);
        var bytes = copy.ToArray();

        copy.Position = 0;
        var analysis = await _cvAnalysis.AnalyzeAsync(copy, fileName, contentType, length, cancellationToken);

        var text = _extractor.ExtractText(bytes);
        var profile = _skillExtractor.Extract(text);

        var request = new JobSearchRequest
        {
            Query = searchRequest.Query,
            Location = searchRequest.Location,
            ExperienceLevel = searchRequest.ExperienceLevel,
            WorkArrangement = searchRequest.WorkArrangement,
            EmploymentType = searchRequest.EmploymentType,
            DatePosted = searchRequest.DatePosted,
            Sort = JobSortOption.HighestMatch,
            MinMatchScore = searchRequest.MinMatchScore,
            QueryRequired = false
        };

        var jobs = await _provider.SearchAsync(request, cancellationToken);
        var matches = jobs
            .Select(job => _matchEngine.Score(profile, job, request.Query, request.Location))
            .Where(match => PassesScoreFilter(match, request.MinMatchScore))
            .OrderByDescending(match => match.EstimatedMatchPercent)
            .ThenByDescending(match => match.Job.PostedDate ?? DateTimeOffset.MinValue)
            .ToList();

        return new JobMatchResponse
        {
            Cv = analysis,
            Results = new JobSearchResponse
            {
                Total = matches.Count,
                Query = request.Query?.Trim() ?? string.Empty,
                Location = request.Location?.Trim(),
                UsingDemoData = string.Equals(_provider.SourceName, "Demo", StringComparison.OrdinalIgnoreCase),
                Jobs = matches.Select(match => JobMapper.ToResponse(match.Job, match)).ToList()
            }
        };
    }

    private static bool PassesScoreFilter(JobMatch match, MatchScoreFilter filter)
    {
        return filter switch
        {
            MatchScoreFilter.EightyPlus => match.EstimatedMatchPercent >= 80,
            MatchScoreFilter.SixtyPlus => match.EstimatedMatchPercent >= 60,
            _ => true
        };
    }
}
