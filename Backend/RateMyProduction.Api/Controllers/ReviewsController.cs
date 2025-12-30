// Api/Controllers/ReviewsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RateMyProduction.Core.DTOs.Requests;
using RateMyProduction.Core.DTOs.Responses;
using RateMyProduction.Core.Interfaces;
using System.Security.Claims;

namespace RateMyProduction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not authenticated"));

    [HttpPost("production/{productionId}")]
    public async Task<ActionResult<ReviewDto>> CreateReview(int productionId, CreateReviewRequest request)
    {
        // TODO: get real userId from JWT later
        var review = await _reviewService.CreateReviewAsync(CurrentUserId, productionId, request);

        return CreatedAtAction(
            nameof(GetReview),
            new { id = review.ReviewID },
            review);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, UpdateReviewRequest request)
    {
        await _reviewService.UpdateReviewAsync(CurrentUserId, id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _reviewService.DeleteReviewAsync(CurrentUserId, id);
        return NoContent();
    }

    [HttpGet("production/{productionId}")]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetReviewsForProduction(
        int productionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var parameters = new ReviewQueryParameters(page, pageSize, CurrentUserId);
        var result = await _reviewService.GetReviewsForProductionAsync(productionId, parameters);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetReview(int id)
    {
        var review = await _reviewService.GetByIdAsync(id, CurrentUserId);

        if (review == null)
            return NotFound();

        return Ok(review);
    }
}