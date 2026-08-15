using JobScout.Application.DTOs;
using JobScout.Domain.Models;

namespace JobScout.Application.Interfaces;

public interface IJobSearchProvider
{
    string SourceName { get; }

    Task<IReadOnlyList<Job>> SearchAsync(
        JobSearchRequest request,
        CancellationToken cancellationToken = default);

    Task<Job?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}
