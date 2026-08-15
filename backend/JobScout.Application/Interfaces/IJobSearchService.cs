using JobScout.Application.DTOs;

namespace JobScout.Application.Interfaces;

public interface IJobSearchService
{
    Task<JobSearchResponse> SearchAsync(
        JobSearchRequest request,
        CancellationToken cancellationToken = default);

    Task<JobResponse?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
