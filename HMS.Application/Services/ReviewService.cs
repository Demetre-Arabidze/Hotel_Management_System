using HMS.Application.Contracts.Persistence;
using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using HMS.Application.Models.Review;
using HMS.Domain.Entities;
using Mapster;

namespace HMS.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepositoryBase<Review> _reviewRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IReservationRepository _reservationRepository; // Optional: verify guest stayed

        public ReviewService(
            IRepositoryBase<Review> reviewRepository,
            IHotelRepository hotelRepository,
            IReservationRepository reservationRepository)
        {
            _reviewRepository = reviewRepository;
            _hotelRepository = hotelRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<ReviewResponseDto> AddReviewAsync(Guid hotelId, Guid userId, CreateReviewDto dto)
        {
            // 1. Check if hotel exists
            var hotel = await _hotelRepository.GetAsync(h => h.Id == hotelId, tracking: true);
            if (hotel == null)
                throw new NotFoundException(nameof(Hotel), hotelId);

            // 2. (Optional Domain Check) Ensure user actually stayed at this hotel before reviewing
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            bool hasStayed = await _reservationRepository.ExistsAsync(r =>
                r.GuestId == userId &&
                r.CheckOutDate <= today &&
                r.ReservationRooms.Any(rr => rr.Room.HotelId == hotelId));

            if (!hasStayed)
                throw new BadRequestException("Only guests who have completed a stay can leave a review.");

            // 3. Create review entity
            var review = dto.Adapt<Review>();
            review.Id = Guid.NewGuid();
            review.HotelId = hotelId;
            review.UserId = userId;
            review.CreatedAt = DateTime.UtcNow;

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveAsync();

            // 4. Recalculate average rating for the hotel
            var (allReviews, _) = await _reviewRepository.GetAllAsync(r => r.HotelId == hotelId);
            if (allReviews.Any())
            {
                hotel.Rating = Math.Round((decimal)allReviews.Average(r => r.Rating), 1);
                await _hotelRepository.SaveAsync();
            }

            return review.Adapt<ReviewResponseDto>();
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetHotelReviewsAsync(Guid hotelId)
        {
            var (reviews, _) = await _reviewRepository.GetAllAsync(r => r.HotelId == hotelId);
            return reviews.OrderByDescending(r => r.CreatedAt).Adapt<List<ReviewResponseDto>>();
        }

        public async Task UpdateReviewAsync(Guid reviewId, Guid userId, UpdateReviewDto dto)
        {
            var review = await _reviewRepository.GetAsync(r => r.Id == reviewId, tracking: true);
            if (review == null)
                throw new NotFoundException(nameof(Review), reviewId);

            // Ownership Check
            if (review.UserId != userId)
                throw new UnauthorizedAccessException("You are only allowed to update your own review.");

            dto.Adapt(review);
            await _reviewRepository.SaveAsync();

            await RecalculateHotelRatingAsync(review.HotelId);
        }

        public async Task DeleteReviewAsync(Guid reviewId, Guid userId, bool isAdmin = false)
        {
            var review = await _reviewRepository.GetAsync(r => r.Id == reviewId, tracking: true);
            if (review == null)
                throw new NotFoundException(nameof(Review), reviewId);

            // Ownership or Admin Check
            if (review.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("You are only allowed to delete your own review.");

            var hotelId = review.HotelId;

            _reviewRepository.Remove(review);
            await _reviewRepository.SaveAsync();

            await RecalculateHotelRatingAsync(hotelId);
        }

        private async Task RecalculateHotelRatingAsync(Guid hotelId)
        {
            var hotel = await _hotelRepository.GetAsync(h => h.Id == hotelId, tracking: true);
            if (hotel == null) return;

            var (allReviews, _) = await _reviewRepository.GetAllAsync(r => r.HotelId == hotelId);
            if (allReviews.Any())
            {
                hotel.Rating = Math.Round((decimal)allReviews.Average(r => r.Rating), 1);
            }
            else
            {
                hotel.Rating = 0.0m;
            }

            await _hotelRepository.SaveAsync();
        }
    }
}
