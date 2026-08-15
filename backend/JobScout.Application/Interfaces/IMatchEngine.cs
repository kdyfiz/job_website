using JobScout.Domain.Models;

namespace JobScout.Application.Interfaces;

public interface IMatchEngine
{
    JobMatch Score(CandidateProfile profile, Job job, string? query, string? location);
}
