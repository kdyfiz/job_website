using JobScout.Application.DTOs;
using JobScout.Application.Interfaces;
using JobScout.Domain.Models;
using Microsoft.Extensions.Logging;

namespace JobScout.Infrastructure.JobProviders;

public sealed class CompositeJobSearchProvider : IJobSearchProvider
{
    private readonly HimalayasJobSearchProvider _live;
    private readonly DemoJobSearchProvider _demo;
    private readonly ILogger<CompositeJobSearchProvider> _logger;

    public string SourceName => "Himalayas";

    public CompositeJobSearchProvider(
        HimalayasJobSearchProvider live,
        DemoJobSearchProvider demo,
        ILogger<CompositeJobSearchProvider> logger)
    {
        _live = live;
        _demo = demo;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Job>> SearchAsync(
        JobSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var live = await _live.SearchAsync(request, cancellationToken);
            if (live.Count > 0)
            {
                return live;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Live Himalayas search failed; using demo jobs.");
        }

        return await _demo.SearchAsync(request, cancellationToken);
    }

    public async Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var live = await _live.GetByIdAsync(id, cancellationToken);
        if (live is not null)
        {
            return live;
        }

        return await _demo.GetByIdAsync(id, cancellationToken);
    }
}
