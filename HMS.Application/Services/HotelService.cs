using HMS.Application.Contracts.Persistence;
using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using HMS.Application.Models.Hotel;
using HMS.Domain.Entities;
using Mapster;

namespace HMS.Application.Services
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IRepositoryBase<Manager> _managerRepository; // Inject Manager repo

        public HotelService(
            IHotelRepository hotelRepository,
            IReservationRepository reservationRepository,
            IRepositoryBase<Manager> managerRepository) // Update constructor
        {
            _hotelRepository = hotelRepository;
            _reservationRepository = reservationRepository;
            _managerRepository = managerRepository;
        }

        public async Task<IEnumerable<HotelResponseDto>> GetAllAsync()
        {
            var (hotels, totalCount) = await _hotelRepository.GetAllAsync();
            return hotels.Adapt<List<HotelResponseDto>>();
        }

        public async Task<HotelWithManagerDto> GetByIdWithDetailsAsync(Guid id)
        {
            var hotel = await _hotelRepository.GetByIdWithDetailsAsync(id);
            if (hotel == null)
                throw new NotFoundException(nameof(Hotel), id);

            return hotel.Adapt<HotelWithManagerDto>();
        }

        public async Task<HotelResponseDto> GetByIdAsync(Guid id)
        {
            var hotel = await _hotelRepository.GetAsync(h => h.Id == id, tracking: false);
            if (hotel == null)
                throw new NotFoundException(nameof(Hotel), id);

            return hotel.Adapt<HotelResponseDto>();
        }

        public async Task<HotelResponseDto> CreateAsync(HotelCreateUpdateDto dto)
        {
            if (await _hotelRepository.ExistsAsync(h => h.Name == dto.Name))
                throw new BadRequestException($"Hotel with name '{dto.Name}' already exists.");

            var hotel = dto.Adapt<Hotel>();
            hotel.Rating = 0.0m;
            hotel.Id = Guid.NewGuid();

            await _hotelRepository.AddAsync(hotel);
            await _hotelRepository.SaveAsync();

            return hotel.Adapt<HotelResponseDto>();
        }

        public async Task UpdateAsync(Guid id, HotelCreateUpdateDto dto, Guid userId, bool isAdmin)
        {
            // 1. Check if the hotel exists
            var hotel = await _hotelRepository.GetAsync(h => h.Id == id, tracking: true);
            if (hotel == null)
                throw new NotFoundException(nameof(Hotel), id);

            // 2. DOMAIN SECURITY CHECK: If not Admin, ensure Manager owns this hotel
            if (!isAdmin)
            {
                var manager = await _managerRepository.GetAsync(m => m.UserId == userId);
                if (manager == null || manager.HotelId != id)
                {
                    // Throws an exception that your Global Handler will eventually catch
                    throw new UnauthorizedAccessException("You are only authorized to update your assigned hotel.");
                }
            }

            // 3. Prevent duplicate names
            if (await _hotelRepository.ExistsAsync(h => h.Name == dto.Name && h.Id != id))
                throw new BadRequestException($"Another hotel with name '{dto.Name}' already exists.");

            // 4. Update and save
            dto.Adapt(hotel);
            await _hotelRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var hotel = await _hotelRepository.GetAsync(h => h.Id == id, tracking: false);
            if (hotel == null)
                throw new NotFoundException(nameof(Hotel), id);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            bool hasActiveBookings = await _reservationRepository.ExistsAsync(r =>
                r.ReservationRooms.Any(rr => rr.Room.HotelId == id) && r.CheckOutDate >= today);

            if (hasActiveBookings)
                throw new BadRequestException("Cannot delete hotel with active or upcoming reservations.");

            _hotelRepository.Remove(hotel);
            await _hotelRepository.SaveAsync();
        }
    }

}
