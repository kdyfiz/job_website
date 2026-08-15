using JobScout.Application.Interfaces;
using JobScout.Application.Options;
using JobScout.Application.Services;
using JobScout.Application.Validators;
using JobScout.Infrastructure.CV;
using JobScout.Infrastructure.JobProviders;
using JobScout.Infrastructure.Matching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobScout.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddJobScoutInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MatchingOptions>(configuration.GetSection(MatchingOptions.SectionName));
        services.Configure<CvOptions>(configuration.GetSection(CvOptions.SectionName));
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.SectionName));

        services.AddSingleton(SkillCatalog.LoadFromEmbeddedResource());
        services.AddSingleton<IJobSearchProvider, DemoJobSearchProvider>();
        services.AddSingleton<IPdfTextExtractor, DocnetPdfTextExtractor>();
        services.AddSingleton<ISkillExtractor, SkillExtractor>();
        services.AddSingleton<IMatchEngine, MatchEngine>();
        services.AddSingleton<CvUploadValidator>();

        services.AddScoped<IJobSearchService, JobSearchService>();
        services.AddScoped<ICvAnalysisService, CvAnalysisService>();
        services.AddScoped<IJobMatchService, JobMatchService>();

        return services;
    }
}
