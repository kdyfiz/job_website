using JobScout.Application.DTOs;
using JobScout.Infrastructure.JobProviders;
using Microsoft.Extensions.Logging.Abstractions;

namespace JobScout.UnitTests;

public class CompositeJobSearchProviderTests
{
    [Fact]
    public async Task Falls_back_to_demo_when_live_returns_nothing()
    {
        var live = new HimalayasJobSearchProvider(
            new HttpClient(new EmptyHandler()) { BaseAddress = new Uri("https://himalayas.app") },
            NullLogger<HimalayasJobSearchProvider>.Instance);
        var composite = new CompositeJobSearchProvider(
            live,
            new DemoJobSearchProvider(),
            NullLogger<CompositeJobSearchProvider>.Instance);

        var results = await composite.SearchAsync(new JobSearchRequest
        {
            Query = "Junior Software Developer",
            Location = "Malaysia",
            QueryRequired = true
        });

        Assert.NotEmpty(results);
        Assert.All(results, job => Assert.Equal("Demo", job.Source));
    }

    private sealed class EmptyHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("""{"jobs":[]}""")
            });
        }
    }
}
