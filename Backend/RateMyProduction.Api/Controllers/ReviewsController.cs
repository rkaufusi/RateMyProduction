// Api/Controllers/ReviewsController.cs
using Microsoft.AspNetCore.Mvc;
using RateMyProduction.Core.Interfaces;
using RateMyProduction.Core.Services; // for DTOs (since they're in the interface file)

namespace RateMyProduction.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly int _currentUserId = 1;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost("production/{productionId}")]
    public async Task<ActionResult<ReviewDto>> CreateReview(int productionId, CreateReviewRequest request)
    {
        // TODO: get real userId from JWT later
        var review = await _reviewService.CreateReviewAsync(_currentUserId, productionId, request);

        return CreatedAtAction(
            nameof(GetReview),
            new { id = review.ReviewID },
            review);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, UpdateReviewRequest request)
    {
        await _reviewService.UpdateReviewAsync(_currentUserId, id, request);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _reviewService.DeleteReviewAsync(_currentUserId, id);
        return NoContent();
    }

    [HttpGet("production/{productionId}")]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetReviewsForProduction(
        int productionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var parameters = new ReviewQueryParameters(page, pageSize, _currentUserId);
        var result = await _reviewService.GetReviewsForProductionAsync(productionId, parameters);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetReview(int id)
    {
        throw new NotImplementedException("Add GetById to service or load via production");
    }
}