using HMS.Application.Contracts.Services;
using HMS.Application.Models.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/hotels/{hotelId:guid}/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(Guid hotelId)
        {
            var reviews = await _reviewService.GetHotelReviewsAsync(hotelId);
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize(Roles = "Guest")] // Requires authenticated user (Guest)
        public async Task<IActionResult> CreateReview(Guid hotelId, [FromBody] CreateReviewDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized("Invalid token.");

            var result = await _reviewService.AddReviewAsync(hotelId, userId, dto);
            return CreatedAtAction(nameof(GetReviews), new { hotelId }, result);
        }

        [HttpPut("{reviewId:guid}")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> UpdateReview(Guid hotelId, Guid reviewId, [FromBody] UpdateReviewDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized("Invalid token.");

            await _reviewService.UpdateReviewAsync(reviewId, userId, dto);
            return NoContent();
        }

        [HttpDelete("{reviewId:guid}")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> DeleteReview(Guid hotelId, Guid reviewId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized("Invalid token.");

            bool isAdmin = User.IsInRole("Admin");

            await _reviewService.DeleteReviewAsync(reviewId, userId, isAdmin);
            return NoContent();
        }
    }
}
