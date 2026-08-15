using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using JobScout.Application.DTOs;

namespace JobScout.IntegrationTests;

public class JobsApiTests : IClassFixture<JobsApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public JobsApiTests(JobsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_returns_ok()
    {
        var response = await _client.GetAsync("/api/health");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("ok", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Valid_search_returns_jobs()
    {
        var response = await _client.GetAsync("/api/jobs/search?query=software%20developer&location=Malaysia");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<JobSearchResponse>(JsonOptions);
        Assert.NotNull(payload);
        Assert.True(payload!.Total > 0);
        Assert.True(payload.UsingDemoData);
    }

    [Fact]
    public async Task Missing_query_returns_bad_request()
    {
        var response = await _client.GetAsync("/api/jobs/search");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions);
        Assert.Equal("validation_error", payload?.Error.Code);
    }

    [Fact]
    public async Task Unknown_job_returns_not_found()
    {
        var response = await _client.GetAsync("/api/jobs/does-not-exist");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Known_job_returns_details()
    {
        var response = await _client.GetAsync("/api/jobs/demo-001");
        response.EnsureSuccessStatusCode();
        var job = await response.Content.ReadFromJsonAsync<JobResponse>(JsonOptions);
        Assert.Equal("Junior Software Developer", job?.Title);
        Assert.True(job?.IsDemoData);
    }

    [Fact]
    public async Task Cv_analyze_without_file_returns_bad_request()
    {
        var response = await _client.PostAsync("/api/cv/analyze", new MultipartFormDataContent());
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
