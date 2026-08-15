using JobScout.Application.DTOs;
using JobScout.Domain.Models;

namespace JobScout.Application.Mapping;

public static class JobMapper
{
    public static JobResponse ToResponse(Job job, JobMatch? match = null)
    {
        var isDemo = string.Equals(job.Source, "Demo", StringComparison.OrdinalIgnoreCase);

        return new JobResponse
        {
            Id = job.Id,
            Title = job.Title,
            Company = job.Company,
            Location = job.Location,
            Description = job.Description,
            EmploymentType = job.EmploymentType,
            ExperienceLevel = job.ExperienceLevel,
            WorkArrangement = job.WorkArrangement,
            Skills = job.Skills,
            Salary = job.Salary,
            PostedDate = job.PostedDate,
            Source = job.Source,
            SourceUrl = job.SourceUrl,
            AvailabilityStatus = job.AvailabilityStatus,
            IsDemoData = isDemo,
            EstimatedMatchPercent = match?.EstimatedMatchPercent,
            Match = match is null
                ? null
                : new MatchExplanationResponse
                {
                    EstimatedMatchPercent = match.EstimatedMatchPercent,
                    MatchingSkills = match.Breakdown.MatchingSkills,
                    MissingSkills = match.Breakdown.MissingSkills,
                    ExperienceExplanation = match.Breakdown.ExperienceExplanation
                }
        };
    }
}
