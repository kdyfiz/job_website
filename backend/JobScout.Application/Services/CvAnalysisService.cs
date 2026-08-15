using JobScout.Application.DTOs;
using JobScout.Application.Exceptions;
using JobScout.Application.Interfaces;
using JobScout.Application.Validators;
using Microsoft.Extensions.Logging;

namespace JobScout.Application.Services;

public sealed class CvAnalysisService : ICvAnalysisService
{
    private readonly CvUploadValidator _validator;
    private readonly IPdfTextExtractor _extractor;
    private readonly ISkillExtractor _skillExtractor;
    private readonly ILogger<CvAnalysisService> _logger;

    public CvAnalysisService(
        CvUploadValidator validator,
        IPdfTextExtractor extractor,
        ISkillExtractor skillExtractor,
        ILogger<CvAnalysisService> logger)
    {
        _validator = validator;
        _extractor = extractor;
        _skillExtractor = skillExtractor;
        _logger = logger;
    }

    public async Task<CVAnalysisResponse> AnalyzeAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long length,
        CancellationToken cancellationToken = default)
    {
        await using var buffer = new MemoryStream();
        await fileStream.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();

        var header = bytes.Length >= 8 ? bytes[..8] : bytes;
        var errors = _validator.Validate(fileName, contentType, length > 0 ? length : bytes.Length, header);
        if (errors.Count > 0)
        {
            throw new JobScoutException("cv_invalid", errors[0]);
        }

        string text;
        try
        {
            text = _extractor.ExtractText(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CV text extraction failed");
            throw new JobScoutException(
                "cv_unreadable",
                "We couldn't detect readable text in this CV. Please upload a text-based PDF.");
        }

        if (string.IsNullOrWhiteSpace(text) || text.Trim().Length < 20)
        {
            throw new JobScoutException(
                "cv_unreadable",
                "We couldn't detect readable text in this CV. Please upload a text-based PDF.");
        }

        var profile = _skillExtractor.Extract(text);

        return new CVAnalysisResponse
        {
            SkillCount = profile.Skills.Count,
            Skills = profile.Skills,
            ExperienceIndicators = profile.ExperienceIndicators,
            Warning = profile.Skills.Count == 0
                ? "We couldn't detect known skills in this CV. Matching will be limited."
                : null
        };
    }
}
