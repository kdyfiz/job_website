using JobScout.Application.DTOs;

namespace JobScout.Application.Validators;

public static class JobSearchRequestValidator
{
    public static IReadOnlyList<string> Validate(JobSearchRequest request)
    {
        var errors = new List<string>();

        if (request.QueryRequired)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                errors.Add("Please enter a job title or keywords.");
            }
            else if (request.Query.Trim().Length < 2)
            {
                errors.Add("Please enter at least 2 characters for the job title.");
            }
        }

        if (request.Query is { Length: > 120 })
        {
            errors.Add("Job title is too long.");
        }

        if (request.Location is { Length: > 120 })
        {
            errors.Add("Location is too long.");
        }

        return errors;
    }
}
