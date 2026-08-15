using JobScout.Application.Exceptions;
using JobScout.Application.Options;
using JobScout.Application.Services;
using JobScout.Application.Validators;
using JobScout.Infrastructure.CV;
using JobScout.UnitTests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace JobScout.UnitTests;

public class CvAnalysisServiceTests
{
    private readonly CvAnalysisService _service = new(
        new CvUploadValidator(Options.Create(new CvOptions())),
        new DocnetPdfTextExtractor(),
        new SkillExtractor(SkillCatalog.LoadFromEmbeddedResource()),
        NullLogger<CvAnalysisService>.Instance);

    [Fact]
    public async Task Valid_pdf_extracts_skills()
    {
        var bytes = PdfFactory.WithText("Java SQL React Python Postman Cypress Fresh Graduate");
        await using var stream = new MemoryStream(bytes);

        var result = await _service.AnalyzeAsync(stream, "cv.pdf", "application/pdf", bytes.Length);

        Assert.Contains("Java", result.Skills);
        Assert.Contains("React", result.Skills);
        Assert.True(result.SkillCount >= 3);
    }

    [Fact]
    public async Task Empty_pdf_returns_unreadable_error()
    {
        var bytes = PdfFactory.Empty();
        await using var stream = new MemoryStream(bytes);

        var ex = await Assert.ThrowsAsync<JobScoutException>(() =>
            _service.AnalyzeAsync(stream, "cv.pdf", "application/pdf", bytes.Length));

        Assert.Equal("cv_unreadable", ex.Code);
    }

    [Fact]
    public async Task Unsupported_file_is_rejected()
    {
        var bytes = "not a pdf"u8.ToArray();
        await using var stream = new MemoryStream(bytes);

        var ex = await Assert.ThrowsAsync<JobScoutException>(() =>
            _service.AnalyzeAsync(stream, "cv.txt", "text/plain", bytes.Length));

        Assert.Equal("cv_invalid", ex.Code);
    }

    [Fact]
    public async Task Oversized_file_is_rejected()
    {
        var bytes = PdfFactory.WithText("Java");
        await using var stream = new MemoryStream(bytes);

        var ex = await Assert.ThrowsAsync<JobScoutException>(() =>
            _service.AnalyzeAsync(stream, "cv.pdf", "application/pdf", 6 * 1024 * 1024));

        Assert.Equal("cv_invalid", ex.Code);
    }
}
