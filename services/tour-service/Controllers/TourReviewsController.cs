using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourService.DTO;
using TourService.Services;

namespace TourService.Controllers;

[Authorize]
public class TourReviewsController : BaseApiController
{
    private readonly ITourReviewService _tourReviewService;

    public TourReviewsController(ITourReviewService tourReviewService)
    {
        _tourReviewService = tourReviewService;
    }

    [HttpPost]
    public async Task<ActionResult<TourReviewDto>> CreateReview([FromBody] CreateTourReviewRequestDto request)
    {
        var result = await _tourReviewService.CreateReviewAsync(request);
        return CreateResponse(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<TourReviewDto>> GetReview(long id)
    {
        var result = await _tourReviewService.GetReviewByIdAsync(id);
        return CreateResponse(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<TourReviewDto>>> GetReviews([FromQuery] long? tourId, [FromQuery] long? userId)
    {
        if (tourId.HasValue && userId.HasValue)
        {
            var result = await _tourReviewService.GetReviewsByTourAndUserAsync(tourId.Value, userId.Value);
            return CreateResponse(result);
        }
        else if (tourId.HasValue)
        {
            var result = await _tourReviewService.GetReviewsByTourIdAsync(tourId.Value);
            return CreateResponse(result);
        }
        else if (userId.HasValue)
        {
            var result = await _tourReviewService.GetReviewsByUserIdAsync(userId.Value);
            return CreateResponse(result);
        }
        else
        {
            var result = await _tourReviewService.GetAllReviewsAsync();
            return CreateResponse(result);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TourReviewDto>> UpdateReview(long id, [FromBody] UpdateTourReviewRequestDto request)
    {
        var userId = GetUserId();
        var result = await _tourReviewService.UpdateReviewAsync(id, request, userId);
        return CreateResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteReview(long id)
    {
        var userId = GetUserId();
        var result = await _tourReviewService.DeleteReviewAsync(id, userId);
        return CreateResponse(result);
    }

    [HttpGet("tour/{tourId}/rating")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> GetTourRating(long tourId)
    {
        var averageRatingResult = await _tourReviewService.GetAverageRatingForTourAsync(tourId);
        var countResult = await _tourReviewService.GetReviewCountForTourAsync(tourId);
        
        if (averageRatingResult.IsFailed)
        {
            return CreateResponse(averageRatingResult);
        }
        
        if (countResult.IsFailed)
        {
            return CreateResponse(countResult);
        }

        return Ok(new
        {
            TourId = tourId,
            AverageRating = averageRatingResult.Value,
            ReviewCount = countResult.Value
        });
    }
}