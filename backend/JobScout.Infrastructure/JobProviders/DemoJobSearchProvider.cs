using System.Text.Json;
using System.Text.Json.Serialization;
using JobScout.Application.DTOs;
using JobScout.Application.Interfaces;
using JobScout.Domain.Enums;
using JobScout.Domain.Models;
using JobScout.Infrastructure.Matching;

namespace JobScout.Infrastructure.JobProviders;

public sealed class DemoJobSearchProvider : IJobSearchProvider
{
    private readonly IReadOnlyList<Job> _jobs;

    public string SourceName => "Demo";

    public DemoJobSearchProvider()
    {
        _jobs = LoadJobs();
    }

    public Task<IReadOnlyList<Job>> SearchAsync(
        JobSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Job> results = _jobs;

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var query = request.Query.Trim();
            results = results.Where(job =>
                job.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                job.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                job.Company.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                job.Skills.Any(skill => skill.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        results = results.Where(job =>
            LocationMatcher.Matches(job.Location, request.EffectiveLocation, job.WorkArrangement));

        if (request.ExperienceLevel != ExperienceLevel.Any)
        {
            results = results.Where(job =>
                job.ExperienceLevel is null ||
                job.ExperienceLevel == request.ExperienceLevel);
        }

        if (request.WorkArrangement != WorkArrangement.Any)
        {
            results = results.Where(job =>
                job.WorkArrangement is null ||
                job.WorkArrangement == request.WorkArrangement);
        }

        if (request.EmploymentType != EmploymentType.Any)
        {
            results = results.Where(job =>
                job.EmploymentType is null ||
                job.EmploymentType == request.EmploymentType);
        }

        if (request.DatePosted != DatePostedFilter.Any)
        {
            var cutoff = DateTimeOffset.UtcNow.AddDays(-(int)request.DatePosted);
            results = results.Where(job => job.PostedDate is not null && job.PostedDate >= cutoff);
        }

        return Task.FromResult<IReadOnlyList<Job>>(results.ToList());
    }

    public Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var job = _jobs.FirstOrDefault(j => j.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(job);
    }

    private static IReadOnlyList<Job> LoadJobs()
    {
        var assembly = typeof(DemoJobSearchProvider).Assembly;
        var name = assembly.GetManifestResourceNames()
            .First(n => n.EndsWith("demo-jobs.json", StringComparison.OrdinalIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException("demo-jobs.json embedded resource was not found.");
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var records = JsonSerializer.Deserialize<List<DemoJobRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("demo-jobs.json could not be parsed.");

        var now = DateTimeOffset.UtcNow.Date;
        return records.Select(record => new Job
        {
            Id = record.Id,
            Title = record.Title,
            Company = record.Company,
            Location = record.Location,
            Description = record.Description,
            EmploymentType = record.EmploymentType,
            ExperienceLevel = record.ExperienceLevel,
            WorkArrangement = record.WorkArrangement,
            Skills = record.Skills,
            Salary = string.IsNullOrWhiteSpace(record.SalaryDisplay)
                ? null
                : new SalaryInfo { Display = record.SalaryDisplay },
            PostedDate = now.AddDays(-record.PostedDaysAgo),
            Source = "Demo",
            SourceUrl = null,
            AvailabilityStatus = record.AvailabilityStatus
        }).ToList();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private sealed class DemoJobRecord
    {
        public required string Id { get; init; }
        public required string Title { get; init; }
        public required string Company { get; init; }
        public required string Location { get; init; }
        public required string Description { get; init; }
        public EmploymentType? EmploymentType { get; init; }
        public ExperienceLevel? ExperienceLevel { get; init; }
        public WorkArrangement? WorkArrangement { get; init; }
        public List<string> Skills { get; init; } = [];
        public string? SalaryDisplay { get; init; }
        public int PostedDaysAgo { get; init; }
        public AvailabilityStatus AvailabilityStatus { get; init; }
    }
}
