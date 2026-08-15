using JobScout.Application.DTOs;

namespace JobScout.Application.Interfaces;

public interface ICvAnalysisService
{
    Task<CVAnalysisResponse> AnalyzeAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long length,
        CancellationToken cancellationToken = default);
}
