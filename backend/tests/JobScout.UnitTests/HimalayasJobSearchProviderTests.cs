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
            {"jobs":[{"title":"Backend Engineer","companyName":"Globex","description":"C#","employmentType":"Full Time","seniority":["Mid Level"],"locationRestrictions":[],"categories":["backend"],"applicationLink":"https://example.com/apply","guid":"g1","pubDate":1700000000}]}
            """;
        var provider = CreateProvider(json);

        var results = await provider.SearchAsync(new JobSearchRequest
        {
            Query = "engineer",
            QueryRequired = true
        });

        var job = Assert.Single(results);
        Assert.Equal("Backend Engineer", job.Title);
        Assert.Equal("Himalayas", job.Source);

        var byId = await provider.GetByIdAsync(job.Id);
        Assert.Equal(job.Title, byId?.Title);
    }

    [Fact]
    public async Task On_site_filter_returns_no_live_jobs()
    {
        var provider = CreateProvider("""{"jobs":[{"title":"Remote Dev","companyName":"Acme"}]}""");

        var results = await provider.SearchAsync(new JobSearchRequest
        {
            Query = "dev",
            WorkArrangement = WorkArrangement.OnSite,
            QueryRequired = true
        });

        Assert.Empty(results);
    }

    private static HimalayasJobSearchProvider CreateProvider(string json)
    {
        var handler = new StubHandler(json);
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://himalayas.app")
        };
        return new HimalayasJobSearchProvider(http, NullLogger<HimalayasJobSearchProvider>.Instance);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly string _json;

        public StubHandler(string json) => _json = json;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json, Encoding.UTF8, "application/json")
            });
        }
    }
}
