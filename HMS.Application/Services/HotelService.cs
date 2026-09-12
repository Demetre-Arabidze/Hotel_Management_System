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
        private readonly IRepositoryBase<Hotel> _hotelRepository;
        private readonly IReservationRepository _reservationRepository;

        public HotelService(
            IRepositoryBase<Hotel> hotelRepository,
            IReservationRepository reservationRepository)
        {
            _hotelRepository = hotelRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<HotelResponseDto>> GetAllAsync()
        {
            var hotels = await _hotelRepository.GetAllAsync(tracking: false);
            return hotels.Adapt<IEnumerable<HotelResponseDto>>();
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
            hotel.Id = Guid.NewGuid();

            await _hotelRepository.AddAsync(hotel);
            await _hotelRepository.SaveAsync();

            return hotel.Adapt<HotelResponseDto>();
        }

        public async Task UpdateAsync(Guid id, HotelCreateUpdateDto dto)
        {
            var hotel = await _hotelRepository.GetAsync(h => h.Id == id, tracking: true);
            if (hotel == null)
                throw new NotFoundException(nameof(Hotel), id);

            if (await _hotelRepository.ExistsAsync(h => h.Name == dto.Name && h.Id != id))
                throw new BadRequestException($"Another hotel with name '{dto.Name}' already exists.");

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
