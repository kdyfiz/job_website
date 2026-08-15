using JobScout.Application.DTOs;

namespace JobScout.Application.Interfaces;

public interface IJobMatchService
{
    Task<JobMatchResponse> MatchAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        long length,
        JobSearchRequest searchRequest,
        CancellationToken cancellationToken = default);
}
