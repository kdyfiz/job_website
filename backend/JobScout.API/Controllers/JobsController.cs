using JobScout.Application.DTOs;
using JobScout.Application.Interfaces;
using JobScout.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace JobScout.API.Controllers;

[ApiController]
[Route("api/jobs")]
[EnableRateLimiting("search")]
public sealed class JobsController : ControllerBase
{
    private readonly IJobSearchService _searchService;

    public JobsController(IJobSearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(JobSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobSearchResponse>> Search(
        [FromQuery] string? query,
        [FromQuery] string? location,
        [FromQuery] ExperienceLevel experienceLevel = ExperienceLevel.Any,
        [FromQuery] WorkArrangement workArrangement = WorkArrangement.Any,
        [FromQuery] EmploymentType employmentType = EmploymentType.Any,
        [FromQuery] DatePostedFilter datePosted = DatePostedFilter.Any,
        [FromQuery] JobSortOption sort = JobSortOption.MostRelevant,
        CancellationToken cancellationToken = default)
    {
        var request = new JobSearchRequest
        {
            Query = query,
            Location = location,
            ExperienceLevel = experienceLevel,
            WorkArrangement = workArrangement,
            EmploymentType = employmentType,
            DatePosted = datePosted,
            Sort = sort,
            QueryRequired = true
        };

        var result = await _searchService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        var job = await _searchService.GetByIdAsync(id, cancellationToken);
        if (job is null)
        {
            return NotFound(new ApiErrorResponse
            {
                Error = new ApiError
                {
                    Code = "job_not_found",
                    Message = "We couldn't find that job listing."
                }
            });
        }

        return Ok(job);
    }
}
