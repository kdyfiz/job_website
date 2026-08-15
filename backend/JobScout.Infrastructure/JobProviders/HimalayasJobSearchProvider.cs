using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using JobScout.Application.DTOs;
using JobScout.Application.Interfaces;
using JobScout.Domain.Enums;
using JobScout.Domain.Models;
using Microsoft.Extensions.Logging;

namespace JobScout.Infrastructure.JobProviders;

public sealed class HimalayasJobSearchProvider : IJobSearchProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly ILogger<HimalayasJobSearchProvider> _logger;
    private readonly ConcurrentDictionary<string, Job> _cache = new(StringComparer.OrdinalIgnoreCase);

    public string SourceName => "Himalayas";

    public HimalayasJobSearchProvider(HttpClient http, ILogger<HimalayasJobSearchProvider> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Job>> SearchAsync(
        JobSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.WorkArrangement == WorkArrangement.OnSite)
        {
            return [];
        }

        var query = string.IsNullOrWhiteSpace(request.Query) ? "software" : request.Query.Trim();
        var jobs = await FetchAsync(query, includeMalaysia: true, cancellationToken);

        if (jobs.Count == 0)
        {
            jobs = await FetchAsync(query, includeMalaysia: false, cancellationToken);
        }

        foreach (var job in jobs)
        {
            _cache[job.Id] = job;
        }

        IEnumerable<Job> results = jobs;

        if (request.ExperienceLevel != ExperienceLevel.Any)
        {
            results = results.Where(job =>
                job.ExperienceLevel is null ||
                job.ExperienceLevel == request.ExperienceLevel);
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

        return results.ToList();
    }

    public Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(id, out var job);
        return Task.FromResult(job);
    }

    private async Task<IReadOnlyList<Job>> FetchAsync(
        string query,
        bool includeMalaysia,
        CancellationToken cancellationToken)
    {
        var path = includeMalaysia
            ? $"/jobs/api/search?q={Uri.EscapeDataString(query)}&country=MY&sort=recent&page=1"
            : $"/jobs/api/search?q={Uri.EscapeDataString(query)}&sort=recent&page=1";

        using var response = await _http.GetAsync(path, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Himalayas returned {Status} for {Path}", (int)response.StatusCode, path);
            return [];
        }

        var payload = await response.Content.ReadFromJsonAsync<HimalayasSearchResponse>(JsonOptions, cancellationToken);
        if (payload?.Jobs is null)
        {
            return [];
        }

        return payload.Jobs.Select(Map).Where(job => job is not null).Cast<Job>().ToList();
    }

    internal static Job? Map(HimalayasJob record)
    {
        if (string.IsNullOrWhiteSpace(record.Title) || string.IsNullOrWhiteSpace(record.CompanyName))
        {
            return null;
        }

        var sourceUrl = record.ApplicationLink ?? record.Guid;
        var id = "himalayas-" + StableId(sourceUrl ?? record.Title);
        var description = PlainText(
            string.IsNullOrWhiteSpace(record.Description) ? record.Excerpt : record.Description);
        if (description.Length == 0)
        {
            description = record.Title.Trim();
        }

        return new Job
        {
            Id = id,
            Title = record.Title.Trim(),
            Company = record.CompanyName.Trim(),
            Location = FormatLocation(record.LocationRestrictions),
            Description = description,
            EmploymentType = MapEmployment(record.EmploymentType),
            ExperienceLevel = MapSeniority(record.Seniority),
            WorkArrangement = WorkArrangement.Remote,
            Skills = (record.Categories ?? [])
                .Select(category => category.Replace('-', ' '))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(8)
                .ToList(),
            Salary = FormatSalary(record),
            PostedDate = record.PubDate is > 0
                ? DateTimeOffset.FromUnixTimeSeconds(record.PubDate.Value)
                : null,
            Source = "Himalayas",
            SourceUrl = sourceUrl,
            AvailabilityStatus = AvailabilityStatus.AppearsActive
        };
    }

    private static string PlainText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        var text = Regex.Replace(html, @"<(br|p|div|li)\s*/?>", "\n", RegexOptions.IgnoreCase);
        text = Regex.Replace(text, "<[^>]+>", " ");
        return WebUtility.HtmlDecode(text).Trim();
    }

    private static string StableId(string value)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(hash)[..12].ToLowerInvariant();
    }

    private static string FormatLocation(IReadOnlyList<string>? restrictions)
    {
        if (restrictions is null || restrictions.Count == 0)
        {
            return "Remote";
        }

        if (restrictions.Any(place => place.Contains("Malaysia", StringComparison.OrdinalIgnoreCase)))
        {
            return "Malaysia (Remote)";
        }

        var shown = string.Join(", ", restrictions.Take(3));
        return restrictions.Count > 3 ? $"{shown} (Remote)" : $"{shown} (Remote)";
    }

    private static EmploymentType? MapEmployment(string? value) => value?.Trim() switch
    {
        "Full Time" => EmploymentType.FullTime,
        "Part Time" => EmploymentType.PartTime,
        "Intern" or "Internship" => EmploymentType.Internship,
        "Contractor" or "Contract" or "Temporary" => EmploymentType.Contract,
        _ => null
    };

    private static ExperienceLevel? MapSeniority(IReadOnlyList<string>? seniority)
    {
        if (seniority is null || seniority.Count == 0)
        {
            return null;
        }

        var text = string.Join(' ', seniority);
        if (text.Contains("Intern", StringComparison.OrdinalIgnoreCase)) return ExperienceLevel.Internship;
        if (text.Contains("Entry", StringComparison.OrdinalIgnoreCase)) return ExperienceLevel.EntryLevel;
        if (text.Contains("Mid", StringComparison.OrdinalIgnoreCase)) return ExperienceLevel.OneToTwoYears;
        if (text.Contains("Senior", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("Manager", StringComparison.OrdinalIgnoreCase))
        {
            return ExperienceLevel.ThreeToFiveYears;
        }

        return null;
    }

    private static SalaryInfo? FormatSalary(HimalayasJob record)
    {
        if (record.MinSalary is null && record.MaxSalary is null)
        {
            return null;
        }

        var currency = string.IsNullOrWhiteSpace(record.Currency) ? "USD" : record.Currency;
        var period = string.IsNullOrWhiteSpace(record.SalaryPeriod) ? "year" : record.SalaryPeriod;
        var display = record.MinSalary is not null && record.MaxSalary is not null
            ? $"{currency} {record.MinSalary:0}-{record.MaxSalary:0} / {period}"
            : $"{currency} {record.MinSalary ?? record.MaxSalary:0} / {period}";

        return new SalaryInfo
        {
            Min = record.MinSalary,
            Max = record.MaxSalary,
            Currency = currency,
            Period = period,
            Display = display
        };
    }

    internal sealed class HimalayasSearchResponse
    {
        public List<HimalayasJob>? Jobs { get; set; }
    }

    internal sealed class HimalayasJob
    {
        public string? Title { get; set; }
        public string? CompanyName { get; set; }
        public string? Excerpt { get; set; }
        public string? Description { get; set; }
        public string? EmploymentType { get; set; }
        public List<string>? Seniority { get; set; }
        public List<string>? LocationRestrictions { get; set; }
        public List<string>? Categories { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? Currency { get; set; }
        public string? SalaryPeriod { get; set; }
        public long? PubDate { get; set; }
        public string? ApplicationLink { get; set; }
        public string? Guid { get; set; }
    }
}
