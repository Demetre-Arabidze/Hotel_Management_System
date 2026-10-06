using HMS.Application.Models.Review;

namespace HMS.Application.Contracts.Services
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> AddReviewAsync(Guid hotelId, Guid userId, CreateReviewDto dto);
        Task<IEnumerable<ReviewResponseDto>> GetHotelReviewsAsync(Guid hotelId);
        Task UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewDto dto);
        Task DeleteReviewAsync(Guid reviewId, Guid userId, bool isAdmin = false);
    }
}
