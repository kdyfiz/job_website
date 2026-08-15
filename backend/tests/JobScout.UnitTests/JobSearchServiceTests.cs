using JobScout.Application.DTOs;
using JobScout.Application.Exceptions;
using JobScout.Application.Interfaces;
using JobScout.Application.Services;
using JobScout.Domain.Models;
using JobScout.Infrastructure.JobProviders;
using Microsoft.Extensions.Logging.Abstractions;

namespace JobScout.UnitTests;

public class JobSearchServiceTests
{
    private readonly JobSearchService _service = new(
        new DemoJobSearchProvider(),
        NullLogger<JobSearchService>.Instance);

    [Fact]
    public async Task Valid_search_returns_jobs()
    {
        var result = await _service.SearchAsync(new JobSearchRequest
        {
            Query = "Junior Software Developer",
            Location = "Malaysia",
            QueryRequired = true
        });

        Assert.True(result.Total > 0);
        Assert.True(result.UsingDemoData);
        Assert.All(result.Jobs, job => Assert.True(job.IsDemoData));
        Assert.Contains(result.Jobs, job => job.Title.Contains("Junior Software Developer", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Empty_search_is_invalid()
    {
        var ex = await Assert.ThrowsAsync<JobScoutException>(() =>
            _service.SearchAsync(new JobSearchRequest { Query = " ", QueryRequired = true }));

        Assert.Equal("validation_error", ex.Code);
    }

    [Fact]
    public async Task No_results_returns_empty_list()
    {
        var result = await _service.SearchAsync(new JobSearchRequest
        {
            Query = "Underwater Basket Weaver Chief",
            QueryRequired = true
        });

        Assert.Equal(0, result.Total);
        Assert.Empty(result.Jobs);
    }

    [Fact]
    public async Task Provider_failure_is_wrapped()
    {
        var failing = new JobSearchService(new FailingProvider(), NullLogger<JobSearchService>.Instance);

        var ex = await Assert.ThrowsAsync<JobScoutException>(() =>
            failing.SearchAsync(new JobSearchRequest { Query = "developer", QueryRequired = true }));

        Assert.Equal("provider_failure", ex.Code);
        Assert.Equal(500, ex.StatusCode);
    }

    [Fact]
    public async Task Get_by_id_returns_demo_job()
    {
        var job = await _service.GetByIdAsync("demo-001");
        Assert.NotNull(job);
        Assert.Equal("Junior Software Developer", job!.Title);
        Assert.True(job.IsDemoData);
    }

    private sealed class FailingProvider : IJobSearchProvider
    {
        public string SourceName => "Demo";

        public Task<IReadOnlyList<Job>> SearchAsync(JobSearchRequest request, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("provider down");

        public Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult<Job?>(null);
    }
}
