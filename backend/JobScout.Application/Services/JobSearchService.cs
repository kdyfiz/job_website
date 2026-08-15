using JobScout.Application.DTOs;
using JobScout.Application.Exceptions;
using JobScout.Application.Interfaces;
using JobScout.Application.Mapping;
using JobScout.Application.Validators;
using JobScout.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace JobScout.Application.Services;

public sealed class JobSearchService : IJobSearchService
{
    private readonly IJobSearchProvider _provider;
    private readonly ILogger<JobSearchService> _logger;

    public JobSearchService(IJobSearchProvider provider, ILogger<JobSearchService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task<JobSearchResponse> SearchAsync(
        JobSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var errors = JobSearchRequestValidator.Validate(request);
        if (errors.Count > 0)
        {
            throw new JobScoutException("validation_error", errors[0]);
        }

        IReadOnlyList<Domain.Models.Job> jobs;
        try
        {
            jobs = await _provider.SearchAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job provider {Provider} failed", _provider.SourceName);
            throw new JobScoutException(
                "provider_failure",
                "Something went wrong while searching for jobs.",
                500);
        }

        var sorted = Sort(jobs, request.Sort, request.Query).ToList();

        return new JobSearchResponse
        {
            Total = sorted.Count,
            Query = request.Query?.Trim() ?? string.Empty,
            Location = request.EffectiveLocation,
            UsingDemoData = jobs.Count == 0 || jobs.All(job =>
                string.Equals(job.Source, "Demo", StringComparison.OrdinalIgnoreCase)),
            Jobs = sorted.Select(job => JobMapper.ToResponse(job)).ToList()
        };
    }

    public async Task<JobResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var job = await _provider.GetByIdAsync(id.Trim(), cancellationToken);
        return job is null ? null : JobMapper.ToResponse(job);
    }

    private static IEnumerable<Domain.Models.Job> Sort(
        IEnumerable<Domain.Models.Job> jobs,
        JobSortOption sort,
        string? query)
    {
        return sort switch
        {
            JobSortOption.Newest => jobs.OrderByDescending(j => j.PostedDate ?? DateTimeOffset.MinValue),
            JobSortOption.MostRelevant => jobs
                .OrderByDescending(j => Relevance(j, query))
                .ThenByDescending(j => j.PostedDate ?? DateTimeOffset.MinValue),
            _ => jobs
        };
    }

    private static int Relevance(Domain.Models.Job job, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return 0;
        }

        var q = query.Trim();
        var score = 0;
        if (job.Title.StartsWith(q, StringComparison.OrdinalIgnoreCase)) score += 30;
        else if (job.Title.Contains(q, StringComparison.OrdinalIgnoreCase)) score += 20;
        if (job.Description.Contains(q, StringComparison.OrdinalIgnoreCase)) score += 5;
        if (job.Skills.Any(s => s.Contains(q, StringComparison.OrdinalIgnoreCase))) score += 10;
        return score;
    }
}
