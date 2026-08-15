using System.Net;
using System.Text;
using JobScout.Application.DTOs;
using JobScout.Domain.Enums;
using JobScout.Infrastructure.JobProviders;
using Microsoft.Extensions.Logging.Abstractions;

namespace JobScout.UnitTests;

public class HimalayasJobSearchProviderTests
{
    [Fact]
    public void Map_sets_remote_source_and_apply_link()
    {
        var job = HimalayasJobSearchProvider.Map(new HimalayasJobSearchProvider.HimalayasJob
        {
            Title = "Frontend Engineer",
            CompanyName = "Acme",
            Description = "React and TypeScript.",
            EmploymentType = "Full Time",
            Seniority = ["Senior"],
            LocationRestrictions = ["Malaysia"],
            Categories = ["software-engineering", "frontend"],
            ApplicationLink = "https://himalayas.app/companies/acme/jobs/frontend",
            Guid = "https://himalayas.app/jobs/guid",
            PubDate = 1_700_000_000,
            MinSalary = 40000,
            MaxSalary = 60000,
            Currency = "USD",
            SalaryPeriod = "year"
        });

        Assert.NotNull(job);
        Assert.StartsWith("himalayas-", job!.Id);
        Assert.Equal("Himalayas", job.Source);
        Assert.Equal(WorkArrangement.Remote, job.WorkArrangement);
        Assert.Equal("Malaysia (Remote)", job.Location);
        Assert.Equal(EmploymentType.FullTime, job.EmploymentType);
        Assert.Equal("https://himalayas.app/companies/acme/jobs/frontend", job.SourceUrl);
    }

    [Fact]
    public void Map_skips_incomplete_rows()
    {
        Assert.Null(HimalayasJobSearchProvider.Map(new HimalayasJobSearchProvider.HimalayasJob
        {
            Title = "Only a title"
        }));
    }

    [Fact]
    public async Task Search_returns_mapped_jobs_from_json()
    {
        var json = """
            {"jobs":[{"title":"Backend Engineer","companyName":"Globex","description":"C#","employmentType":"Full Time","seniority":["Mid Level"],"locationRestrictions":["Malaysia"],"categories":["backend"],"applicationLink":"https://example.com/apply","guid":"g1","pubDate":1700000000}]}
            """;
        var (provider, handler) = CreateProvider(json);

        var results = await provider.SearchAsync(new JobSearchRequest
        {
            Query = "engineer",
            QueryRequired = true
        });

        var job = Assert.Single(results);
        Assert.Equal("Backend Engineer", job.Title);
        Assert.Equal("Himalayas", job.Source);
        Assert.Equal("Malaysia (Remote)", job.Location);
        Assert.Contains("country=MY", handler.LastPath, StringComparison.Ordinal);

        var byId = await provider.GetByIdAsync(job.Id);
        Assert.Equal(job.Title, byId?.Title);
    }

    [Fact]
    public async Task On_site_filter_returns_no_live_jobs()
    {
        var (provider, _) = CreateProvider("""{"jobs":[{"title":"Remote Dev","companyName":"Acme"}]}""");

        var results = await provider.SearchAsync(new JobSearchRequest
        {
            Query = "dev",
            WorkArrangement = WorkArrangement.OnSite,
            QueryRequired = true
        });

        Assert.Empty(results);
    }

    [Fact]
    public async Task Search_drops_jobs_not_open_to_malaysia()
    {
        var json = """
            {"jobs":[
              {"title":"Worldwide Dev","companyName":"Globex","description":"Go","locationRestrictions":[],"applicationLink":"https://example.com/world","guid":"w1"},
              {"title":"US Dev","companyName":"Acme","description":"Go","locationRestrictions":["United States"],"applicationLink":"https://example.com/us","guid":"u1"},
              {"title":"MY Dev","companyName":"Local","description":"Go","locationRestrictions":["Malaysia"],"applicationLink":"https://example.com/my","guid":"m1"}
            ]}
            """;
        var (provider, _) = CreateProvider(json);

        var results = await provider.SearchAsync(new JobSearchRequest
        {
            Query = "dev",
            QueryRequired = true
        });

        var job = Assert.Single(results);
        Assert.Equal("MY Dev", job.Title);
        Assert.Equal("Malaysia (Remote)", job.Location);
    }

    private static (HimalayasJobSearchProvider Provider, StubHandler Handler) CreateProvider(string json)
    {
        var handler = new StubHandler(json);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://himalayas.app")
        };
        return (new HimalayasJobSearchProvider(http, NullLogger<HimalayasJobSearchProvider>.Instance), handler);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly string _json;

        public StubHandler(string json) => _json = json;

        public string LastPath { get; private set; } = string.Empty;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            LastPath = request.RequestUri?.PathAndQuery ?? string.Empty;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json, Encoding.UTF8, "application/json")
            });
        }
    }
}
