using JobScout.Application.DTOs;
using JobScout.Application.Interfaces;
using JobScout.Application.Options;
using JobScout.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace JobScout.API.Controllers;

[ApiController]
[Route("api")]
[EnableRateLimiting("cv")]
public sealed class CvController : ControllerBase
{
    private readonly ICvAnalysisService _cvAnalysis;
    private readonly IJobMatchService _jobMatch;
    private readonly CvOptions _cvOptions;

    public CvController(
        ICvAnalysisService cvAnalysis,
        IJobMatchService jobMatch,
        IOptions<CvOptions> cvOptions)
    {
        _cvAnalysis = cvAnalysis;
        _jobMatch = jobMatch;
        _cvOptions = cvOptions.Value;
    }

    [HttpPost("cv/analyze")]
    [RequestSizeLimit(6_291_456)]
    [ProducesResponseType(typeof(CVAnalysisResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CVAnalysisResponse>> Analyze(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        if (file is null)
        {
            return BadRequest(new ApiErrorResponse
            {
                Error = new ApiError
                {
                    Code = "cv_invalid",
                    Message = "Please upload a PDF CV under 5 MB."
                }
            });
        }

        await using var stream = file.OpenReadStream();
        var result = await _cvAnalysis.AnalyzeAsync(
            stream,
            file.FileName,
            file.ContentType ?? string.Empty,
            file.Length,
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("jobs/match")]
    [RequestSizeLimit(6_291_456)]
    [ProducesResponseType(typeof(JobMatchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobMatchResponse>> Match(
        IFormFile? file,
        [FromForm] string? query,
        [FromForm] string? location,
        [FromForm] ExperienceLevel experienceLevel = ExperienceLevel.Any,
        [FromForm] WorkArrangement workArrangement = WorkArrangement.Any,
        [FromForm] EmploymentType employmentType = EmploymentType.Any,
        [FromForm] DatePostedFilter datePosted = DatePostedFilter.Any,
        [FromForm] MatchScoreFilter minMatchScore = MatchScoreFilter.Any,
        CancellationToken cancellationToken = default)
    {
        if (file is null)
        {
            return BadRequest(new ApiErrorResponse
            {
                Error = new ApiError
                {
                    Code = "cv_invalid",
                    Message = "Please upload a PDF CV under 5 MB."
                }
            });
        }

        if (file.Length > _cvOptions.MaxFileSizeBytes)
        {
            return BadRequest(new ApiErrorResponse
            {
                Error = new ApiError
                {
                    Code = "cv_invalid",
                    Message = "Please upload a PDF CV under 5 MB."
                }
            });
        }

        var search = new JobSearchRequest
        {
            Query = query,
            Location = location,
            ExperienceLevel = experienceLevel,
            WorkArrangement = workArrangement,
            EmploymentType = employmentType,
            DatePosted = datePosted,
            MinMatchScore = minMatchScore,
            QueryRequired = false
        };

        await using var stream = file.OpenReadStream();
        var result = await _jobMatch.MatchAsync(
            stream,
            file.FileName,
            file.ContentType ?? string.Empty,
            file.Length,
            search,
            cancellationToken);

        return Ok(result);
    }
}
